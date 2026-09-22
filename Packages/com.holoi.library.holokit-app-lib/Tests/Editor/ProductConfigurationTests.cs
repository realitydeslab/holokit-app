using System;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoloKit.Tests
{
    public sealed class ProductConfigurationTests
    {
        [Test]
        public void ProductOwnsItsSettingsAndCatalogs()
        {
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            Assert.That(settings, Is.Not.Null);
            foreach (var asset in new UnityEngine.Object[] { settings, settings.AppConfig, settings.RealityList,
                         settings.AvatarCollectionList, settings.ObjectCollectionList })
            {
                Assert.That(asset, Is.Not.Null);
                Assert.That(AssetDatabase.GetAssetPath(asset), Does.StartWith("Assets/"));
            }
            var enabled = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path)).ToArray();
            foreach (var reality in settings.GetAllRealities())
                Assert.That(enabled, Does.Contain(reality.Scene.SceneName), reality.name);
        }

        [Test]
        public void RuntimePreferencesDoNotMutateProductDefaults()
        {
            var template = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            var runtime = HoloKitProductConfiguration.CreateRuntimeSettings(template);
            try
            {
                Assert.That(runtime, Is.Not.SameAs(template));
                runtime.WatermarkEnabled = !template.WatermarkEnabled;
                Assert.That(runtime.WatermarkEnabled, Is.Not.EqualTo(template.WatermarkEnabled));
            }
            finally { UnityEngine.Object.DestroyImmediate(runtime); }
        }

        [Test]
        public void MissingProductConfigurationFailsBeforeBootstrap()
        {
            Assert.Throws<InvalidOperationException>(() => HoloKitProductConfiguration.CreateRuntimeSettings(null));
        }
    }
}
