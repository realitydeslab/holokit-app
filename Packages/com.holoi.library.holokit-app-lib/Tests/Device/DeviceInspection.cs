#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HoloKit.DeviceTests
{
    /// <summary>
    /// Renders content on the device from a virtual camera aimed at it, so a test can check that an
    /// object draws (and what it looks like) without anyone pointing the phone.
    /// </summary>
    public static class DeviceInspection
    {
        [Serializable]
        public sealed class Result
        {
            /// <summary>Fraction of pixels that change when the object is hidden: what the object itself draws.</summary>
            public float objectPixels;
            /// <summary>Mean colour (0-255) of the changed pixels.</summary>
            public Color32 meanColor;
            /// <summary>Mean HSV saturation (0-1) of the changed pixels; near 0 means grey or white.</summary>
            public float meanSaturation;
            public string withPath;
            public string withoutPath;
        }

        /// <summary>
        /// Renders <paramref name="target"/> from <paramref name="distance"/> metres with and without
        /// <paramref name="toggle"/> enabled, with time frozen so only the toggled parts differ.
        /// The virtual camera copies the main camera's settings and renders several frames first,
        /// because VFX Graph only draws for cameras it has prepared.
        /// </summary>
        public static IEnumerator Capture(Vector3 target, float distance, IEnumerable<Component> toggle,
            string directory, string name, Action<Result> done, int size = 720)
        {
            var main = Camera.main;
            var behaviours = toggle.Where(c => c != null && IsEnabled(c)).ToList();
            var texture = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
            var inspector = new GameObject("Device Inspection Camera").AddComponent<Camera>();
            var timeScale = Time.timeScale;
            var result = new Result
            {
                withPath = Path.Combine(directory, name + ".png"),
                withoutPath = Path.Combine(directory, name + "-hidden.png")
            };
            try
            {
                if (main != null) inspector.CopyFrom(main);
                inspector.targetTexture = texture;
                inspector.clearFlags = CameraClearFlags.SolidColor;
                inspector.backgroundColor = Color.black;
                inspector.nearClipPlane = 0.01f;
                var from = main != null ? Vector3.ProjectOnPlane(target - main.transform.position, Vector3.up) : Vector3.forward;
                if (from.sqrMagnitude < 1e-4f) from = Vector3.forward;
                inspector.transform.position = target - from.normalized * distance + Vector3.up * 0.2f;
                inspector.transform.LookAt(target);
                for (var frame = 0; frame < 10; frame++) yield return new WaitForEndOfFrame();
                Time.timeScale = 0f;
                for (var frame = 0; frame < 3; frame++) yield return new WaitForEndOfFrame();
                var with = Read(texture);
                behaviours.ForEach(c => SetEnabled(c, false));
                for (var frame = 0; frame < 3; frame++) yield return new WaitForEndOfFrame();
                var without = Read(texture);
                behaviours.ForEach(c => SetEnabled(c, true));
                File.WriteAllBytes(result.withPath, with.EncodeToPNG());
                File.WriteAllBytes(result.withoutPath, without.EncodeToPNG());
                Compare(with, without, result);
                UnityEngine.Object.Destroy(with);
                UnityEngine.Object.Destroy(without);
            }
            finally
            {
                Time.timeScale = timeScale;
                behaviours.ForEach(c => { if (c != null) SetEnabled(c, true); });
                UnityEngine.Object.Destroy(inspector.gameObject);
                texture.Release();
            }
            done(result);
        }

        private static bool IsEnabled(Component c) => c is Renderer r ? r.enabled : c is Behaviour b && b.enabled;

        private static void SetEnabled(Component c, bool value)
        {
            if (c is Renderer r) r.enabled = value;
            else if (c is Behaviour b) b.enabled = value;
        }

        private static Texture2D Read(RenderTexture source)
        {
            var active = RenderTexture.active;
            RenderTexture.active = source;
            var texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            texture.Apply();
            RenderTexture.active = active;
            return texture;
        }

        private static void Compare(Texture2D with, Texture2D without, Result result)
        {
            var a = with.GetPixels32();
            var b = without.GetPixels32();
            long r = 0, g = 0, bl = 0, count = 0;
            float saturation = 0f;
            for (var i = 0; i < a.Length; i++)
            {
                if (Math.Abs(a[i].r - b[i].r) + Math.Abs(a[i].g - b[i].g) + Math.Abs(a[i].b - b[i].b) <= 30) continue;
                count++;
                r += a[i].r; g += a[i].g; bl += a[i].b;
                Color.RGBToHSV(a[i], out _, out var s, out _);
                saturation += s;
            }
            result.objectPixels = (float)count / a.Length;
            if (count == 0) return;
            result.meanColor = new Color32((byte)(r / count), (byte)(g / count), (byte)(bl / count), 255);
            result.meanSaturation = saturation / count;
        }
    }
}
#endif
