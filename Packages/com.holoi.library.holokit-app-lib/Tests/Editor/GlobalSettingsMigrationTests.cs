using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.GameTests
{
    public sealed class GlobalSettingsMigrationTests
    {
        private string _directory;
        private HoloKitAppGlobalSettings _settings;
        private string LegacyPath => Path.Combine(_directory, "HoloKitAppGlobalSettings.save");
        private string JsonPath => Path.Combine(_directory, "HoloKitAppGlobalSettings.json");

        [SetUp] public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), "holokit-save-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
            _settings = ScriptableObject.CreateInstance<HoloKitAppGlobalSettings>();
            _settings.InstructionEnabled = false;
            _settings.VibrationEnabled = true;
            _settings.PhaseEnabled = false;
            _settings.HighResHDREnabled = true;
            _settings.RecordMicrophone = false;
            _settings.WatermarkEnabled = true;
            _settings.UseWifiForMultiplayerEnabled = true;
            _settings.ShowTechInfoEnabled = false;
            _settings.RealityPreferences = new Dictionary<string, RealityPreference>
            {
                ["test.reality"] = new RealityPreference
                {
                    MetaAvatarCollectionBundleId = "test.avatars", MetaAvatarTokenId = "42",
                    MetaObjectCollectionBundleId = "test.objects", MetaObjectTokenId = "7"
                }
            };
        }

        [TearDown] public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_settings);
            if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
        }

        [Test] public void JsonRoundTripPreservesOptionsAndSelections()
        {
            HoloKitAppSaveSystem.SaveToDirectory(_directory, new HoloKitAppGlobalSettingsData(_settings));
            AssertMatches(HoloKitAppSaveSystem.LoadFromDirectory(_directory));
            Assert.That(File.ReadAllText(JsonPath), Does.Not.Contain("Holoi.Library.HoloKitApp"));
        }

        [Test] public void SavingAgainRetainsPreviousDocument()
        {
            HoloKitAppSaveSystem.SaveToDirectory(_directory, new HoloKitAppGlobalSettingsData(_settings));
            var first = File.ReadAllText(JsonPath);
            _settings.RecordMicrophone = true;
            HoloKitAppSaveSystem.SaveToDirectory(_directory, new HoloKitAppGlobalSettingsData(_settings));
            Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory).RecordMicrophone, Is.True);
            Assert.That(File.ReadAllText(JsonPath + ".previous"), Is.EqualTo(first));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LegacyBinaryMigrationPreservesValuesAndOriginalFile(bool oldAssemblyNames)
        {
            WriteLegacy(new HoloKitAppGlobalSettingsData(_settings), oldAssemblyNames);
            var original = File.ReadAllBytes(LegacyPath);
            AssertMatches(HoloKitAppSaveSystem.LoadFromDirectory(_directory));
            Assert.That(File.ReadAllBytes(LegacyPath), Is.EqualTo(original));
            Assert.That(File.Exists(JsonPath), Is.True);
            AssertMatches(HoloKitAppSaveSystem.LoadFromDirectory(_directory));
            // An explicitly supplied real-device fixture supplements the synthetic App Store graph.
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-legacySettingsFixture");
            if (oldAssemblyNames && index >= 0 && index + 1 < args.Length)
            {
                File.Delete(JsonPath);
                File.Copy(args[index + 1], LegacyPath, true);
                var source = File.ReadAllBytes(LegacyPath);
                var migrated = HoloKitAppSaveSystem.LoadFromDirectory(_directory);
                Assert.That(migrated, Is.Not.Null);
                Assert.That(migrated.RealityPreferences, Is.Not.Empty);
                Assert.That(File.ReadAllBytes(LegacyPath), Is.EqualTo(source));
                Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory).RealityPreferences.Keys,
                    Is.EquivalentTo(migrated.RealityPreferences.Keys));
            }
        }

        [Test] public void ConcurrentSavesToOneDirectoryDoNotFail()
        {
            // Several app instances (e.g. multi-Editor tests) can share one data directory.
            var data = new HoloKitAppGlobalSettingsData(_settings);
            var failures = new System.Collections.Concurrent.ConcurrentBag<Exception>();
            System.Threading.Tasks.Parallel.For(0, 8, _ =>
            {
                for (var i = 0; i < 25; i++)
                {
                    try { HoloKitAppSaveSystem.SaveToDirectory(_directory, data); }
                    catch (IOException error) { failures.Add(error); }
                }
            });
            Assert.That(failures, Is.Empty, failures.FirstOrDefault()?.ToString());
            Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory), Is.Not.Null);
        }

        [Test] public void MalformedJsonIsPreservedBeforeLegacyFallback()
        {
            WriteLegacy(new HoloKitAppGlobalSettingsData(_settings), true);
            File.WriteAllText(JsonPath, "{not-json");
            LogAssert.Expect(LogType.Warning, new Regex(@"\[SaveSystem\] Preserved unreadable settings"));
            AssertMatches(HoloKitAppSaveSystem.LoadFromDirectory(_directory));
            var backup = Directory.GetFiles(_directory, "*.unreadable-*").Single();
            Assert.That(File.ReadAllText(backup), Is.EqualTo("{not-json"));
        }

        [Test] public void TruncatedLegacyFileIsRetainedWithoutCrashingStartup()
        {
            File.WriteAllBytes(LegacyPath, new byte[] { 0, 1, 2, 3 });
            LogAssert.Expect(LogType.Warning, new Regex(@"\[SaveSystem\] Cannot read legacy settings"));
            Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory), Is.Null);
            Assert.That(File.ReadAllBytes(LegacyPath), Is.EqualTo(new byte[] { 0, 1, 2, 3 }));
        }

        [Test] public void LegacyNullPreferencesBecomeAnEmptyDictionary()
        {
            var data = new HoloKitAppGlobalSettingsData(_settings) { RealityPreferences = null };
            WriteLegacy(data, true);
            Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory).RealityPreferences, Is.Empty);
        }

        [Test] public void UnexpectedLegacyObjectGraphIsRejected()
        {
            WriteLegacy(new List<int> { 1, 2 }, false);
            LogAssert.Expect(LogType.Warning, new Regex(@"\[SaveSystem\] Cannot read legacy settings"));
            Assert.That(HoloKitAppSaveSystem.LoadFromDirectory(_directory), Is.Null);
            Assert.That(File.Exists(LegacyPath), Is.True);
        }

        private void AssertMatches(HoloKitAppGlobalSettingsData actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.InstructionEnabled, Is.EqualTo(_settings.InstructionEnabled));
            Assert.That(actual.VibrationEnabled, Is.EqualTo(_settings.VibrationEnabled));
            Assert.That(actual.PhaseEnabled, Is.EqualTo(_settings.PhaseEnabled));
            Assert.That(actual.HighResHDREnabled, Is.EqualTo(_settings.HighResHDREnabled));
            Assert.That(actual.RecordMicrophone, Is.EqualTo(_settings.RecordMicrophone));
            Assert.That(actual.WatermarkEnabled, Is.EqualTo(_settings.WatermarkEnabled));
            Assert.That(actual.UseWifiForMultiplayerEnabled, Is.EqualTo(_settings.UseWifiForMultiplayerEnabled));
            Assert.That(actual.ShowTechInfoEnabled, Is.EqualTo(_settings.ShowTechInfoEnabled));
            Assert.That(actual.RealityPreferences, Is.EquivalentTo(_settings.RealityPreferences));
        }

        private void WriteLegacy(object data, bool oldNames)
        {
            using var stream = File.Create(LegacyPath);
#pragma warning disable SYSLIB0011
            var formatter = new BinaryFormatter();
            if (oldNames) formatter.Binder = new AppStoreNamesBinder();
            formatter.Serialize(stream, data);
#pragma warning restore SYSLIB0011
        }

        private sealed class AppStoreNamesBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName) => throw new NotSupportedException();
            public override void BindToName(Type type, out string assemblyName, out string typeName)
            {
                assemblyName = type.Assembly.FullName.Replace("Holoi.Library.HoloKitAppLib", "Holoi.Library.HoloKitApp");
                typeName = type.FullName.Replace("Holoi.Library.HoloKitAppLib", "Holoi.Library.HoloKitApp");
            }
        }
    }
}
