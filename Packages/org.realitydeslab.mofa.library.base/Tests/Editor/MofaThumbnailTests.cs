using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>The reality thumbnails show the wands they showed from January to December 2023.</summary>
    public class MofaThumbnailTests
    {
        // The hand effect the in-game AI player holds; the only wand a thumbnail has ever held.
        private const string WandPath = "Packages/org.realitydeslab.mofa.object.spells/Assets/WeaponsVFX/_prefabs/ancient art vfx.prefab";

        // Who held it from 01e272fb (2023-01-05) until "Remove Weapons" (60f9645f, 2023-12-10) cleared the Meebits prefabs'
        // default wand: the Duel's 5958 and both Hunting avatars. Every other thumbnail avatar is empty-handed, as then.
        // Keyed by reality asset name and avatar prefab name.
        private static readonly Dictionary<string, string[]> Holders = new()
        {
            ["MOFATheDuel"] = new[] { "5958" },
            ["MOFATheHunting"] = new[] { "5958", "13173" },
        };

        private static bool Holds(string reality, string avatar) =>
            Holders.TryGetValue(reality, out var avatars) && avatars.Contains(avatar.Split(' ')[0]);

        // The Meebits prefabs no longer carry a wand, so each thumbnail gives one to the avatars that hold it.
        // Checks the thumbnails the realities in this project point to, which in the MOFA app are its own covers.
        [Test]
        public void ThumbnailAvatarsHoldTheir2023Wands()
        {
            var wrong = new List<string>();
            var avatars = 0;
            foreach (var path in AssetDatabase.FindAssets("t:Reality").Select(AssetDatabase.GUIDToAssetPath))
            {
                using var reality = new SerializedObject(AssetDatabase.LoadMainAssetAtPath(path));
                if (reality.FindProperty("ThumbnailPrefab")?.objectReferenceValue is not GameObject thumbnail) continue;
                foreach (var avatar in Avatars(thumbnail))
                {
                    avatars++;
                    using var serialized = new SerializedObject(avatar);
                    var held = serialized.FindProperty("_holdWeapon").boolValue;
                    var weapon = serialized.FindProperty("WeaponPrefab").objectReferenceValue;
                    var holdsWand = held && AssetDatabase.GetAssetPath(weapon) == WandPath;
                    var holdsAnything = held && weapon != null;
                    var expected = Holds(Path.GetFileNameWithoutExtension(path), avatar.name);
                    if (expected ? !holdsWand : holdsAnything)
                        wrong.Add($"{thumbnail.name}/{avatar.name}: holds {(holdsAnything ? weapon.name : "nothing")}, " +
                                  $"expected {(expected ? Path.GetFileNameWithoutExtension(WandPath) : "nothing")} ({path})");
                }
            }
            Assert.That(avatars, Is.GreaterThan(0), "No reality thumbnail with a Meebits avatar was found.");
            Assert.That(wrong, Is.Empty, "Thumbnail avatars that differ from 2023:\n" + string.Join("\n", wrong));
        }

        // An iPhone 15 Pro screen, which the gallery's orthographic camera fills.
        private const int Width = 1179, Height = 2556;
        // Fewer changed pixels than this and the wand is not drawn; a formed one changes about 500 to 2500.
        private const int MinWandPixels = 40;
        // The gallery's room spacing and camera pose relative to a room (HoloKitAppUIPanel_RealityGalleryPage).
        private const float RoomSpacing = 13.5f;
        private static readonly Vector3 CameraOffset = new(-10f, 18f, -8.8f);
        private static readonly Quaternion CameraRotation = Quaternion.Euler(48f, 48f, 0f);

        /// <summary>
        /// Opens the reality gallery as the app does and renders each room the way the gallery shows it, with and
        /// without each avatar's wand in the same frame; the pixels that change are the wand as the player sees it.
        /// </summary>
        [UnityTest, Timeout(300000)]
        public IEnumerator GalleryAvatarsShowTheirWands()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Gallery());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Gallery()
        {
            yield return EditorAppDriver.WaitUntil(() => HoloKitApp.Instance != null, "App bootstrap did not create HoloKitApp.");
            yield return new WaitForSecondsRealtime(1);
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("RealityGalleryPage");
            yield return EditorAppDriver.WaitUntil(() => Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .Any(IsAvatar), "The gallery shows no Meebits avatar.");
            // Each avatar fades in over a second and takes its wand in Start; let the wands' effects form.
            yield return EditorAppDriver.RenderMainCameraFor(2f);

            var camera = Camera.main;
            var directory = EditorAppDriver.OutputDirectory("MofaThumbnails");
            var report = new List<string>();
            var hidden = new List<string>();
            var measured = new List<string>();
            foreach (var room in Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Where(IsAvatar)
                         .GroupBy(avatar => Room(avatar.transform)).OrderBy(group => group.Key.position.x))
            {
                camera.transform.SetPositionAndRotation(room.Key.position + CameraOffset, CameraRotation);
                // As the gallery rests on a room: effects out of view are culled and only simulate once seen.
                yield return EditorAppDriver.RenderMainCameraFor(1.5f);
                var target = RenderTexture.GetTemporary(Width, Height, 24, RenderTextureFormat.ARGB32);
                var withWands = Capture(camera, target);
                File.WriteAllBytes(Path.Combine(directory, room.Key.name + ".png"), Encode(withWands, Width, Height));
                // The room as it is now: a wand is compared with a frame from the same moment, never an older one.
                var frame = withWands;
                foreach (var avatar in room)
                {
                    var wand = avatar.GetType().GetProperty("WeaponInstance")?.GetValue(avatar) as GameObject;
                    var label = $"{room.Key.name}/{avatar.name}";
                    if (wand == null) continue;
                    measured.Add($"{room.Key.name.Replace("(Clone)", "")}/{avatar.name.Split(' ')[0]}");
                    // Some poses put the hand behind the avatar's own hair; the gallery loops the animation, so look again
                    // every half second, up to 3.5 s, until the wand shows.
                    var changed = 0;
                    var looks = 0;
                    while (true)
                    {
                        looks++;
                        // Hide only its renderers: deactivating the wand would restart its effects.
                        changed = frame.Zip(CaptureWithout(wand, camera, target), Differs).Count(d => d);
                        if (changed >= MinWandPixels || looks == 8) break;
                        yield return EditorAppDriver.RenderMainCameraFor(.5f);
                        frame = Capture(camera, target);
                    }
                    var effects = wand.GetComponentsInChildren<VisualEffect>(true);
                    var screen = Viewport(camera, target, wand.transform.position);
                    report.Add($"{label}: {changed} px changed at look {looks}; {effects.Length} effects, " +
                               $"{effects.Sum(e => e.aliveParticleCount)} particles alive, at screen ({screen.x * Width:0}, {screen.y * Height:0})");
                    SaveCrop(frame, screen, Path.Combine(directory, $"{room.Key.name}-{avatar.name}-hand.png"));
                    if (changed < MinWandPixels) hidden.Add($"{label}: {changed} px");
                }
                RenderTexture.ReleaseTemporary(target);
            }
            File.WriteAllLines(Path.Combine(directory, "wands.txt"), report);
            Debug.Log("[MofaThumbnails]\n" + string.Join("\n", report));
            var expected = Holders.SelectMany(h => h.Value.Select(avatar => $"{h.Key}/{avatar}"));
            Assert.That(measured, Is.EquivalentTo(expected), "The gallery's wands are not the 2023 ones.");
            Assert.That(hidden, Is.Empty, $"Gallery avatars whose wand shows fewer than {MinWandPixels} pixels on an iPhone 15 Pro " +
                                          $"screen (frames in {directory}):\n" + string.Join("\n", hidden));
        }

        private static IEnumerable<MonoBehaviour> Avatars(GameObject root) => root.GetComponentsInChildren<MonoBehaviour>(true).Where(IsAvatar);

        private static bool IsAvatar(MonoBehaviour component) => component != null && component.GetType().Name == "MeebitsController";

        // The gallery parents every room to one root object.
        private static Transform Room(Transform child)
        {
            while (child.parent != null && child.parent.parent != null) child = child.parent;
            return child;
        }

        private static bool Differs(Color32 a, Color32 b) => Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) > 24;

        // Projects with the render target's aspect, which the camera only takes while it renders into it.
        private static Vector3 Viewport(Camera camera, RenderTexture target, Vector3 world)
        {
            var previous = camera.targetTexture;
            camera.targetTexture = target;
            var viewport = camera.WorldToViewportPoint(world);
            camera.targetTexture = previous;
            return viewport;
        }

        private static Color32[] Capture(Camera camera, RenderTexture target)
        {
            var previous = camera.targetTexture;
            camera.targetTexture = target;
            camera.Render();
            camera.targetTexture = previous;
            var active = RenderTexture.active;
            RenderTexture.active = target;
            var image = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            RenderTexture.active = active;
            var pixels = image.GetPixels32();
            Object.DestroyImmediate(image);
            return pixels;
        }

        private static Color32[] CaptureWithout(GameObject wand, Camera camera, RenderTexture target)
        {
            var renderers = wand.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToList();
            renderers.ForEach(r => r.enabled = false);
            var pixels = Capture(camera, target);
            renderers.ForEach(r => r.enabled = true);
            return pixels;
        }

        private static byte[] Encode(Color32[] pixels, int width, int height)
        {
            var image = new Texture2D(width, height, TextureFormat.RGBA32, false);
            image.SetPixels32(pixels);
            var png = image.EncodeToPNG();
            Object.DestroyImmediate(image);
            return png;
        }

        // A 160 px square around the hand, enlarged 4x so the wand can be seen.
        private static void SaveCrop(Color32[] frame, Vector3 viewport, string path)
        {
            const int size = 160, zoom = 4;
            int cx = Mathf.RoundToInt(viewport.x * Width), cy = Mathf.RoundToInt(viewport.y * Height);
            var crop = new Color32[size * zoom * size * zoom];
            for (var y = 0; y < size * zoom; y++)
            for (var x = 0; x < size * zoom; x++)
            {
                int sx = Mathf.Clamp(cx - size / 2 + x / zoom, 0, Width - 1), sy = Mathf.Clamp(cy - size / 2 + y / zoom, 0, Height - 1);
                crop[y * size * zoom + x] = frame[sy * Width + sx];
            }
            File.WriteAllBytes(path, Encode(crop, size * zoom, size * zoom));
        }
    }
}
