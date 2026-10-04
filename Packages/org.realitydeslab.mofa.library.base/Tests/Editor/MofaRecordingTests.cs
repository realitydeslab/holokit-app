using System.Collections;
using System.IO;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>What the in-app screen recorder captures of MOFA's magic (rendered the way it records a frame).</summary>
    public sealed class MofaRecordingTests
    {
        private const int Width = 540, Height = 960;

        [UnityTest, Timeout(300000)]
        public IEnumerator ARecordingShowsTheSpellsGlow()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Glow());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Glow()
        {
            yield return EditorAppDriver.EnterReality("MOFATheTraining", true, HoloKitAppPlayerType.Player);
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            var camera = Camera.main;

            // A spell-bright object (an HDR colour, as at a spell's core) 1.5 m in front of the phone. The scene's
            // bloom (threshold 1) makes it glow on screen.
            var spell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(spell.GetComponent<Collider>());
            spell.transform.position = camera.transform.position + camera.transform.forward * 1.5f;
            spell.transform.localScale = Vector3.one * 0.15f;
            var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            material.SetColor("_BaseColor", new Color(6f, 3f, 1f));
            spell.GetComponent<Renderer>().material = material;

            var data = camera.GetComponent<UniversalAdditionalCameraData>();
            var directory = EditorAppDriver.OutputDirectory("MofaRecording");
            var withPost = Record(camera, spell.transform, Path.Combine(directory, "recorded-frame.png"));
            data.renderPostProcessing = false;
            var withoutPost = Record(camera, spell.transform, Path.Combine(directory, "recorded-frame-without-post-processing.png"));
            data.renderPostProcessing = true;
            Object.Destroy(spell);

            Assert.That(withPost - withoutPost, Is.GreaterThan(0.03f),
                $"A recorded frame shows no glow around a spell-bright object: warmth around it {withPost:+0.000;-0.000} with " +
                $"post-processing, {withoutPost:+0.000;-0.000} without (frames in {directory}).");
        }

        /// <summary>
        /// Records one frame as the recorder does and returns how warm (red minus blue) a ring around the object is: the
        /// object's orange glow tints it, while tonemapping only darkens the grey room behind it.
        /// </summary>
        private static float Record(Camera camera, Transform spell, string path)
        {
            var buffer = HoloKitVideoRecorder.CreateCameraBuffer(Width, Height);
            var frame = new RenderTexture(Width, Height, 0);
            try
            {
                var target = camera.targetTexture;
                camera.targetTexture = buffer;
                var centre = camera.WorldToViewportPoint(spell.position);
                var edge = camera.WorldToViewportPoint(spell.position + camera.transform.up * spell.localScale.y * 0.5f);
                camera.targetTexture = target;
                HoloKitVideoRecorder.RenderFrame(camera, buffer, frame);

                var active = RenderTexture.active;
                RenderTexture.active = frame;
                var image = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
                image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
                image.Apply();
                RenderTexture.active = active;
                File.WriteAllBytes(path, image.EncodeToPNG());

                // The recorder flips the frame vertically for the encoder.
                var cx = centre.x * Width;
                var cy = (1f - centre.y) * Height;
                var radius = Mathf.Abs(edge.y - centre.y) * Height;
                float sum = 0f;
                var count = 0;
                for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    var distance = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    if (distance < radius * 1.5f || distance > radius * 3f) continue;
                    var p = image.GetPixel(x, y);
                    sum += p.r - p.b;
                    count++;
                }
                Object.DestroyImmediate(image);
                return count == 0 ? 0f : sum / count;
            }
            finally
            {
                Object.DestroyImmediate(buffer);
                Object.DestroyImmediate(frame);
            }
        }
    }
}
