// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using HoloKit;

[assembly: InternalsVisibleTo("HoloKit.SceneTests.Shared")]

namespace Holoi.Library.HoloKitAppLib
{
    public static class HoloKitAppSaveSystem
    {
        private const string LegacyFileName = "HoloKitAppGlobalSettings.save";
        private const string FileName = "HoloKitAppGlobalSettings.json";

        [Serializable] private sealed class PreferenceEntry
        {
            public string RealityBundleId;
            public RealityPreference Preference;
        }

        [Serializable] private sealed class SettingsDocument
        {
            public int Version;
            public HoloKitAppGlobalSettingsData Settings;
            public PreferenceEntry[] Preferences;
        }

        public static void SaveGlobalSettings(HoloKitAppGlobalSettingsData data)
            => SaveToDirectory(Application.persistentDataPath, data);

        public static HoloKitAppGlobalSettingsData LoadGlobalSettings()
            => HoloKitUtils.IsRuntime ? LoadFromDirectory(Application.persistentDataPath) : null;

        internal static void SaveToDirectory(string directory, HoloKitAppGlobalSettingsData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Directory.CreateDirectory(directory);
            var document = new SettingsDocument
            {
                Version = 1,
                Settings = data,
                Preferences = (data.RealityPreferences ?? new Dictionary<string, RealityPreference>())
                    .Select(pair => new PreferenceEntry { RealityBundleId = pair.Key, Preference = pair.Value }).ToArray()
            };
            var path = Path.Combine(directory, FileName);
            // A unique temporary name keeps concurrent writers (e.g. several Editors sharing the
            // data directory in multiplayer tests) from moving each other's file away.
            var temporary = $"{path}.{Guid.NewGuid():N}.tmp";
            File.WriteAllText(temporary, JsonUtility.ToJson(document, true));
            try
            {
                if (File.Exists(path)) File.Replace(temporary, path, path + ".previous");
                else File.Move(temporary, path);
            }
            catch (IOException) when (File.Exists(path) && File.Exists(temporary))
            {
                // Another writer created or replaced the document meanwhile; the latest write wins.
                File.Replace(temporary, path, null);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
            // The original binary save is deliberately retained for recovery.
        }

        internal static HoloKitAppGlobalSettingsData LoadFromDirectory(string directory)
        {
            var path = Path.Combine(directory, FileName);
            if (File.Exists(path))
            {
                try
                {
                    var document = JsonUtility.FromJson<SettingsDocument>(File.ReadAllText(path));
                    if (document == null || document.Version != 1 || document.Settings == null || document.Preferences == null)
                        throw new SerializationException("Invalid or unsupported settings document.");
                    var preferences = new Dictionary<string, RealityPreference>();
                    foreach (var entry in document.Preferences)
                    {
                        if (entry == null || string.IsNullOrEmpty(entry.RealityBundleId) || preferences.ContainsKey(entry.RealityBundleId))
                            throw new SerializationException("Invalid or duplicate reality preference.");
                        preferences.Add(entry.RealityBundleId, entry.Preference);
                    }
                    document.Settings.RealityPreferences = preferences;
                    return document.Settings;
                }
                catch (Exception exception) when (exception is ArgumentException || exception is SerializationException || exception is IOException)
                {
                    // Keep the unreadable file before a future save writes new defaults.
                    File.Copy(path, path + ".unreadable-" + Guid.NewGuid().ToString("N"));
                    Debug.LogWarning("[SaveSystem] Preserved unreadable settings; trying legacy settings. " + exception.Message);
                }
            }
            var legacyPath = Path.Combine(directory, LegacyFileName);
            if (!File.Exists(legacyPath)) return null;
            try
            {
                using var stream = File.OpenRead(legacyPath);
#pragma warning disable SYSLIB0011
                var formatter = new BinaryFormatter { Binder = new LegacySettingsBinder() };
                var settings = formatter.Deserialize(stream) as HoloKitAppGlobalSettingsData;
#pragma warning restore SYSLIB0011
                if (settings == null) throw new SerializationException("Unexpected legacy settings root type.");
                settings.RealityPreferences ??= new Dictionary<string, RealityPreference>();
                SaveToDirectory(directory, settings);
                Debug.Log("[SaveSystem] Migrated legacy settings to JSON; original binary save retained.");
                return settings;
            }
            catch (Exception exception) when (exception is SerializationException || exception is IOException || exception is ArgumentException)
            {
                Debug.LogWarning("[SaveSystem] Cannot read legacy settings; original file retained. " + exception.Message);
                return null;
            }
        }

        // App Store saves refer to the pre-rename namespace and assembly. Bind only
        // the known settings graph, including its closed dictionary types.
        private sealed class LegacySettingsBinder : SerializationBinder
        {
            private static readonly Type[] AllowedTypes =
            {
                typeof(HoloKitAppGlobalSettingsData), typeof(RealityPreference),
                typeof(Dictionary<string, RealityPreference>),
                typeof(KeyValuePair<string, RealityPreference>),
                typeof(KeyValuePair<string, RealityPreference>[]),
                EqualityComparer<string>.Default.GetType()
            };

            public override Type BindToType(string assemblyName, string typeName)
            {
                var normalized = typeName.Replace("Holoi.Library.HoloKitApp,", "Holoi.Library.HoloKitAppLib,")
                    .Replace("Holoi.Library.HoloKitApp.", "Holoi.Library.HoloKitAppLib.");
                // Mono versions use different implementations for the same default comparer.
                if (normalized == "System.Collections.Generic.InternalStringComparer")
                    return EqualityComparer<string>.Default.GetType();
                foreach (var type in AllowedTypes)
                    if (normalized == type.FullName) return type;
                throw new SerializationException("Unsupported legacy settings type: " + typeName);
            }
        }
    }
}
