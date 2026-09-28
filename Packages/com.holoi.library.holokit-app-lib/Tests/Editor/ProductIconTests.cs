using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace HoloKit.Tests
{
    public sealed class ProductIconTests
    {
        private const string HoloKitBundleId = "com.holoi.holokit.holokit-app";

        // MD5 of the HoloKit app icon file (Assets/Branding/AppIcon/1024.png in the HoloKit project).
        private const string HoloKitIconMd5 = "b87703380288f707482c51aff568ffc7";

        private const string WatchIconPath = "WatchApp/HoloKitWatchApp/Assets.xcassets/AppIcon.appiconset/1024.png";

        [Test]
        public void EveryIosIconExistsAtItsSizeAndIsOpaque()
        {
            var problems = IosIcons().SelectMany(icon => IconProblems(icon.path, icon.size)).ToList();
            Assert.That(IosIcons(), Is.Not.Empty, "The product declares no iOS icons.");
            Assert.That(problems, Is.Empty, string.Join("\n", problems));
        }

        [Test]
        public void ProductOtherThanHoloKitHasItsOwnIcon()
        {
            if (PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS) == HoloKitBundleId)
                Assert.Ignore("This is the HoloKit product.");
            var reused = IosIcons().Where(icon => Md5(icon.path) == HoloKitIconMd5).Select(icon => icon.path).ToList();
            var largest = IosIcons().OrderByDescending(icon => icon.size).First();
            Assert.That(Md5(largest.path), Is.Not.EqualTo(HoloKitIconMd5), "The product still ships the HoloKit icon.");
            Assert.That(reused, Is.Empty, "HoloKit icon files still used: " + string.Join(", ", reused));
        }

        [Test]
        public void WatchIconMatchesTheIphoneIcon()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            if (!settings.AppConfig.WatchCompanionEnabled) Assert.Ignore("This product has no Watch app.");
            Assert.That(File.Exists(WatchIconPath), Is.True, "Missing Watch icon: " + WatchIconPath);
            var iphone = IosIcons().OrderByDescending(icon => icon.size).First();
            Assert.That(iphone.size, Is.EqualTo(1024));
            var watchPixels = Load(WatchIconPath);
            var iphonePixels = Load(iphone.path);
            try
            {
                Assert.That(watchPixels.width, Is.EqualTo(iphonePixels.width));
                var different = watchPixels.GetPixels32().Zip(iphonePixels.GetPixels32(), (a, b) =>
                    Math.Abs(a.r - b.r) > 2 || Math.Abs(a.g - b.g) > 2 || Math.Abs(a.b - b.b) > 2).Count(d => d);
                Assert.That(different, Is.Zero, "The Watch icon differs from the iPhone icon.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(watchPixels);
                UnityEngine.Object.DestroyImmediate(iphonePixels);
            }
        }

        private static (string path, int size)[] IosIcons()
        {
            return PlayerSettings.GetSupportedIconKinds(NamedBuildTarget.iOS)
                .SelectMany(kind => PlayerSettings.GetPlatformIcons(NamedBuildTarget.iOS, kind))
                .Where(icon => icon.GetTexture() != null)
                .Select(icon => (AssetDatabase.GetAssetPath(icon.GetTexture()), icon.width))
                .Distinct().ToArray();
        }

        private static string[] IconProblems(string path, int size)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return new[] { $"Icon file missing for {size}px: {path}" };
            var texture = Load(path);
            try
            {
                if (texture.width != size || texture.height != size)
                    return new[] { $"{path} is {texture.width}x{texture.height}, expected {size}x{size}" };
                return texture.GetPixels32().Any(p => p.a < 255)
                    ? new[] { $"{path} has transparent pixels; App Store icons must be opaque" }
                    : Array.Empty<string>();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static Texture2D Load(string path)
        {
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Assert.That(texture.LoadImage(File.ReadAllBytes(path)), Is.True, "Cannot decode " + path);
            return texture;
        }

        private static string Md5(string path)
        {
            using var md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }
    }
}
