using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppSaveSystem
    {
        private static readonly string s_savePath = Application.persistentDataPath + "/LocalPlayerPreferences.save";

        public static void SaveGlobalSettings(HoloKitAppGlobalSettingsData data)
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(s_savePath, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static HoloKitAppGlobalSettingsData LoadGlobalSettings()
        {
            if (HoloKitHelper.IsRuntime && File.Exists(s_savePath))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(s_savePath, FileMode.Open);
                HoloKitAppGlobalSettingsData data = formatter.Deserialize(stream) as HoloKitAppGlobalSettingsData;
                stream.Close();
                return data;
            }
            else
            {
                Debug.Log($"[SaveSystem] Failed to find save file at {s_savePath}");
                return null;
            }
        }
    }
}
