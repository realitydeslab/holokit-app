using System;
using System.Collections;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace HoloKit.Tests
{
    /// <summary>
    /// Drives the real app in Editor Play Mode with XR Simulation, for game interaction tests.
    /// Call <see cref="OpenStartScene"/>, yield <c>new EnterPlayMode()</c> from the test, then
    /// yield a separate coroutine that uses the helpers here. Entering Play Mode reloads the domain,
    /// and the test method's lambda closures are created before that reload and come back null, so
    /// any lambda or captured local must live in the coroutine started after <c>EnterPlayMode</c>.
    /// </summary>
    public static class EditorAppDriver
    {
        public const float Timeout = 30f;

        public static void OpenStartScene()
        {
            var start = EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled);
            Assert.That(start, Is.Not.Null, "The product has no enabled build scene.");
            EditorSceneManager.OpenScene(start.path, OpenSceneMode.Single);
        }

        /// <summary>Waits for app bootstrap, enters a catalog scene and waits for AR tracking.</summary>
        public static IEnumerator EnterReality(string sceneName, bool isHost, HoloKitAppPlayerType role)
        {
            for (var t = 0f; HoloKitApp.Instance == null && t < Timeout; t += Time.unscaledDeltaTime) yield return null;
            var app = HoloKitApp.Instance;
            Assert.That(app, Is.Not.Null, "App bootstrap did not create HoloKitApp.");
            yield return new WaitForSecondsRealtime(1);
            var reality = app.GlobalSettings.GetAllRealities().FirstOrDefault(r => r.Scene.SceneName == sceneName);
            if (reality == null) Assert.Ignore(sceneName + " is not in this product's catalog.");
            app.CurrentReality = reality;
            app.EnterRealityAs(isHost, role);
            yield return WaitUntil(() => SceneManager.GetActiveScene().name == sceneName, sceneName + " did not load.");
            yield return WaitUntil(() => ARSession.state == ARSessionState.SessionTracking,
                "XR Simulation did not reach SessionTracking.");
            yield return WaitUntil(() => Camera.main != null, sceneName + " has no enabled main camera.");
        }

        /// <summary>
        /// Runs a coroutine and every coroutine it yields. Unity abandons outer coroutines when a nested one
        /// throws, so their finally/using blocks (an E2ETrace saving its timeline, for example) never run; this
        /// disposes every level, innermost first, then rethrows the original failure.
        /// </summary>
        public static IEnumerator Run(IEnumerator body)
        {
            var stack = new System.Collections.Generic.Stack<IEnumerator>();
            stack.Push(body);
            while (stack.Count > 0)
            {
                bool more;
                try
                {
                    more = stack.Peek().MoveNext();
                }
                catch (Exception exception)
                {
                    stack.Pop();   // the coroutine that threw has already run its own finally blocks
                    while (stack.Count > 0) (stack.Pop() as IDisposable)?.Dispose();
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
                    throw;
                }
                if (!more)
                {
                    stack.Pop();
                    continue;
                }
                if (stack.Peek().Current is IEnumerator nested) stack.Push(nested);
                else yield return stack.Peek().Current;
            }
        }

        /// <summary>Enters the first scene of the product's catalog as the host player.</summary>
        public static IEnumerator EnterFirstReality()
        {
            for (var t = 0f; HoloKitApp.Instance == null && t < Timeout; t += Time.unscaledDeltaTime) yield return null;
            Assert.That(HoloKitApp.Instance, Is.Not.Null, "App bootstrap did not create HoloKitApp.");
            var first = HoloKitApp.Instance.GlobalSettings.GetAllRealities().FirstOrDefault();
            Assert.That(first, Is.Not.Null, "The product catalog is empty.");
            yield return EnterReality(first.Scene.SceneName, true, HoloKitAppPlayerType.Player);
        }

        public static IEnumerator WaitUntil(Func<bool> condition, string failure, float timeout = Timeout)
        {
            for (var t = 0f; !condition() && t < timeout; t += Time.unscaledDeltaTime) yield return null;
            Assert.That(condition(), Is.True, failure);
        }

        public static string OutputDirectory(string name)
        {
            var root = Environment.GetEnvironmentVariable("HOLOKIT_PLAYTHROUGH_OUTPUT");
            if (string.IsNullOrEmpty(root))
                root = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs", "EditorPlaythrough"));
            var directory = Path.Combine(root, name);
            Directory.CreateDirectory(directory);
            return directory;
        }

        public readonly struct RenderStats
        {
            public RenderStats(float magenta, float litCenter) { Magenta = magenta; LitCenter = litCenter; }
            /// <summary>Fraction of pixels in Unity's error-shader magenta.</summary>
            public float Magenta { get; }
            /// <summary>Fraction of the central half of the image brighter than the dark background.</summary>
            public float LitCenter { get; }
        }

        /// <summary>Renders the main camera to a PNG.</summary>
        public static RenderStats RenderMainCamera(string path, int width = 540, int height = 960)
        {
            var camera = Camera.main;
            Assert.That(camera, Is.Not.Null, "No main camera to render.");
            return Render(camera, path, width, height);
        }

        /// <summary>
        /// Waits <paramref name="seconds"/> while rendering the main camera every frame. In batch mode no Game
        /// view renders it, so VFX Graph treats effects as unseen and stops simulating them; rendering it lets
        /// effects form as they do on a device.
        /// </summary>
        public static IEnumerator RenderMainCameraFor(float seconds) =>
            RenderMainCameraWhile(start => Time.realtimeSinceStartup - start < seconds);

        /// <summary>
        /// Waits for a condition while rendering the main camera every frame (see <see cref="RenderMainCameraFor"/>),
        /// for steps that depend on effects or animations progressing.
        /// </summary>
        public static IEnumerator WaitUntilRendering(Func<bool> condition, string failure, float timeout = Timeout)
        {
            yield return RenderMainCameraWhile(start => !condition() && Time.realtimeSinceStartup - start < timeout);
            Assert.That(condition(), Is.True, failure);
        }

        /// <summary>As <see cref="RenderMainCameraFor"/>, for a number of frames (works with time frozen).</summary>
        public static IEnumerator RenderMainCameraFrames(int frames)
        {
            var remaining = frames;
            return RenderMainCameraWhile(_ => remaining-- > 0);
        }

        private static IEnumerator RenderMainCameraWhile(Func<float, bool> keepGoing)
        {
            var target = RenderTexture.GetTemporary(270, 480, 24, RenderTextureFormat.ARGB32);
            try
            {
                for (var start = Time.realtimeSinceStartup; keepGoing(start);)
                {
                    if (Camera.main != null) RenderInto(Camera.main, target);
                    yield return null;
                }
            }
            finally
            {
                RenderTexture.ReleaseTemporary(target);
            }
        }

        /// <summary>
        /// Turns the simulated player toward <paramref name="target"/> by rotating the XR Origin about the
        /// camera, as if the player turned their head, and waits for the frame to update.
        /// </summary>
        public static IEnumerator FaceMainCameraTowards(Vector3 target)
        {
            var origin = UnityEngine.Object.FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
            Assert.That(origin, Is.Not.Null, "No XR Origin to turn.");
            var camera = Camera.main.transform;
            var angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(camera.forward, Vector3.up),
                Vector3.ProjectOnPlane(target - camera.position, Vector3.up), Vector3.up);
            origin.RotateAroundCameraUsingOriginUp(angle);
            for (var frame = 0; frame < 10; frame++) yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Points the simulated player's view straight at <paramref name="target"/>, pitch included, as a player
        /// aims the phone. MOFA spells fly along the camera's pitch, so turning alone is not enough to hit.
        /// </summary>
        public static IEnumerator AimMainCameraAt(Vector3 target)
        {
            yield return FaceMainCameraTowards(target);
            var origin = UnityEngine.Object.FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
            var camera = Camera.main.transform;
            var pitch = Vector3.SignedAngle(camera.forward, target - camera.position, camera.right);
            origin.transform.RotateAround(camera.position, camera.right, pitch);
            for (var frame = 0; frame < 5; frame++) yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Moves the simulated player: shifts and turns the XR Origin. Editor peers share one simulated room
        /// and origin, so without this every player stands at the same spot.
        /// </summary>
        public static IEnumerator MovePlayer(Vector3 offset, float yawDegrees)
        {
            var origin = UnityEngine.Object.FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
            Assert.That(origin, Is.Not.Null, "No XR Origin to move.");
            origin.transform.position += offset;
            origin.RotateAroundCameraUsingOriginUp(yawDegrees);
            for (var frame = 0; frame < 10; frame++) yield return new WaitForEndOfFrame();
        }

        /// <summary>The single visible UI element with this name.</summary>
        /// <summary>Whether any active UI element has this name.</summary>
        public static bool IsUiVisible(string name) =>
            UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Any(rect => rect.name == name);

        public static GameObject FindVisibleUi(string name)
        {
            var found = UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(rect => rect.name == name).Select(rect => rect.gameObject).ToList();
            Assert.That(found, Has.Count.EqualTo(1), $"Expected one visible '{name}'.");
            return found[0];
        }

        /// <summary>Presses and releases a UI element the way a finger tap does.</summary>
        public static void Tap(GameObject target)
        {
            var data = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
            ExecuteEvents.ExecuteHierarchy(target, data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(target, data, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.ExecuteHierarchy(target, data, ExecuteEvents.pointerClickHandler);
        }

        /// <summary>
        /// Fraction of pixels that differ between two renders of the same view. Render once with and once
        /// without an object to measure what that object itself contributes, independent of the background.
        /// </summary>
        public static float DifferentPixelFraction(string pathA, string pathB)
        {
            var a = new Texture2D(2, 2);
            var b = new Texture2D(2, 2);
            try
            {
                Assert.That(a.LoadImage(File.ReadAllBytes(pathA)) && b.LoadImage(File.ReadAllBytes(pathB)), Is.True);
                Assert.That(a.width == b.width && a.height == b.height, Is.True, "Renders differ in size.");
                var pa = a.GetPixels32();
                var pb = b.GetPixels32();
                var different = 0;
                for (var i = 0; i < pa.Length; i++)
                    if (Math.Abs(pa[i].r - pb[i].r) + Math.Abs(pa[i].g - pb[i].g) + Math.Abs(pa[i].b - pb[i].b) > 30) different++;
                return (float)different / pa.Length;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(a);
                UnityEngine.Object.DestroyImmediate(b);
            }
        }

        private static RenderStats Render(Camera camera, string path, int width, int height)
        {
            var target = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            RenderInto(camera, target);
            var stats = Read(target, path);
            RenderTexture.ReleaseTemporary(target);
            return stats;
        }

        private static void RenderInto(Camera camera, RenderTexture target)
        {
            var request = new RenderPipeline.StandardRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request))
                RenderPipeline.SubmitRenderRequest(camera, request);
            else
            {
                var previous = camera.targetTexture;
                camera.targetTexture = target;
                camera.Render();
                camera.targetTexture = previous;
            }
        }

        /// <summary>Saves a render target as a PNG and measures it (error-shader magenta, lit centre).</summary>
        public static RenderStats Measure(RenderTexture source, string path) => Read(source, path);

        private static RenderStats Read(RenderTexture source, string path)
        {
            int width = source.width, height = source.height;
            var active = RenderTexture.active;
            RenderTexture.active = source;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            RenderTexture.active = active;
            var pixels = texture.GetPixels32();
            var magenta = (float)pixels.Count(p => p.r > 230 && p.b > 230 && p.g < 40) / pixels.Length;
            int lit = 0, center = 0;
            for (var y = height / 4; y < height * 3 / 4; y++)
            for (var x = width / 4; x < width * 3 / 4; x++, center++)
            {
                var p = pixels[y * width + x];
                if (p.r + p.g + p.b > 180) lit++;
            }
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            return new RenderStats(magenta, (float)lit / center);
        }
    }
}
