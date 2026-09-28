using System;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEngine;

namespace HoloKit.Tests
{
    public sealed class ProductWatchAppSourceTests
    {
        private const string WatchSource = "WatchApp/HoloKitWatchApp";

        // Each product keeps its own Watch icon; everything else must match the shared source.
        private const string ProductOwnedFolder = "Assets.xcassets/AppIcon.appiconset/";

        [Test]
        public void ProductWatchAppMatchesSharedSource()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            if (!settings.AppConfig.WatchCompanionEnabled) Assert.Ignore("This product has no Watch app.");

            var project = Path.GetFullPath(Directory.GetCurrentDirectory());
            var repository = Path.GetFullPath(Path.Combine(project, "..", ".."));
            var isProduct = string.Equals(Path.GetFileName(Path.GetDirectoryName(project)), "Products", StringComparison.Ordinal);
            if (!isProduct) Assert.Ignore("This project hosts the shared Watch app source.");
            var shared = Path.Combine(repository, WatchSource);
            if (!Directory.Exists(shared)) Assert.Ignore("Shared Watch app source not found at " + shared);
            var local = Path.Combine(project, WatchSource);
            Assert.That(Directory.Exists(local), Is.True, "Watch is enabled but the product has no " + WatchSource);

            var sharedFiles = RelativeFiles(shared);
            var localFiles = RelativeFiles(local);
            var problems = sharedFiles.Except(localFiles).Select(f => "missing in product: " + f)
                .Concat(localFiles.Except(sharedFiles).Select(f => "only in product: " + f))
                .Concat(sharedFiles.Intersect(localFiles)
                    .Where(f => !File.ReadAllBytes(Path.Combine(shared, f)).SequenceEqual(File.ReadAllBytes(Path.Combine(local, f))))
                    .Select(f => "differs: " + f))
                .ToList();
            Assert.That(problems, Is.Empty,
                $"{local} has drifted from {shared}. Copy the shared source again:\n" + string.Join("\n", problems));
        }

        private static string[] RelativeFiles(string root)
        {
            return Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                .Select(p => Path.GetRelativePath(root, p).Replace('\\', '/'))
                .Where(p => !p.StartsWith(ProductOwnedFolder, StringComparison.Ordinal) && Path.GetFileName(p) != ".DS_Store")
                .OrderBy(p => p, StringComparer.Ordinal).ToArray();
        }
    }
}
