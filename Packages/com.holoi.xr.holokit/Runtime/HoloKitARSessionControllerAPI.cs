using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace HoloKit {
    public enum ThermalState
    {
        ThermalStateNominal = 0,
        ThermalStateFair = 1,
        ThermalStateSerious = 2,
        ThermalStateCritical = 3
    }

    public enum CameraTrackingState
    {
        NotAvailable = 0,
        LimitedWithReasonNone = 1,
        LimitedWithReasonInitializing = 2,
        LimitedWithReasonExcessiveMotion = 3,
        LimitedWithReasonInsufficientFeatures = 4,
        LimitedWithReasonRelocalizing = 5,
        Normal = 6
    }

    public enum ARWorldMappingStatus
    {
        ARWorldMappingStatusNotAvailable = 0,
        ARWorldMappingStatusLimited = 1,
        ARWorldMappingStatusExtending = 2,
        ARWorldMappingStatusMapped = 3
    }

    public struct ARWorldMapDescription
    {
        public string MapName;
        public string MapFilePath;
        public int MapSizeInBytes;
    }

    public static class HoloKitARSessionControllerAPI
    {
        public static byte[] ARWorldMapData => s_arWorldMapData;

        public static string ARWorldMapName => s_arWorldMapName;

        private static byte[] s_arWorldMapData;

        private static string s_arWorldMapName;

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_InterceptUnityARSessionDelegate(IntPtr ptr);

        [DllImport("__Internal")]
        private static extern int HoloKitSDK_GetThermalState();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_PauseCurrentARSession();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_ResumeCurrentARSession();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SetSessionShouldAttemptRelocalization(bool value);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SetScaningEnvironment(bool value);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_GetCurrentARWorldMap();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SaveCurrentARWorldMapWithName(string mapName);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_GetARWorldMapFromDiskWithName(string mapName, int mapSizeInBytes);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_NullifyCurrentARWorldMap();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_LoadARWorldMapWithData(byte[] mapData, int dataSizeInBytes);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_RelocalizeToLoadedARWorldMap();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SetVideoEnhancementMode(int mode);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_ResetOrigin(float[] position, float[] rotation);

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_RegisterARSessionControllerDelegates(
            Action<int> OnThermalStateChanged,
            Action<int> OnCameraChangedTrackingState,
            Action<int> OnARWorldMapStatusChanged,
            Action OnGotCurrentARWorldMap,
            Action<string, int> OnCurrentARWorldMapSaved,
            Action<bool, string, IntPtr, int> OnGotARWorldMapFromDisk,
            Action OnARWorldMapLoaded,
            Action OnRelocalizationSucceeded,
            Action<double, IntPtr> OnARSessionUpdatedFrame,
            Action<double, IntPtr> OnARSessionUpdatedImageAnchor);

        [DllImport("__Internal")]
        private static extern double HoloKitSDK_GetSystemUptime();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_ResetARSessionFirstFrame();

        [DllImport("__Internal")]
        private static extern double HoloKitSDK_GetScreenBrightness();

        [DllImport("__Internal")]
        private static extern void HoloKitSDK_SetScreenBrightness(double value);

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnThermalStateChangedDelegate(int state)
        {
            OnThermalStateChanged?.Invoke((ThermalState)state);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnCameraChangedTrackingStateDelegate(int state)
        {
            OnCameraChangedTrackingState?.Invoke((CameraTrackingState)state);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnARWorldMapStatusChangedDelegate(int status)
        {
            OnARWorldMapStatusChanged?.Invoke((ARWorldMappingStatus)status);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnGotCurrentARWorldMapDelegate()
        {
            OnGotCurrentARWorldMap?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string, int>))]
        private static void OnCurrentARWorldMapSavedDelegate(string mapName, int mapSizeInBytes)
        {
            OnCurrentARWorldMapSaved?.Invoke(mapName, mapSizeInBytes);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<bool, string, IntPtr, int>))]
        private static void OnGotARWorldMapFromDiskDelegate(bool success, string mapName, IntPtr mapPtr, int mapSizeInBytes)
        {
            if (success)
            {
                s_arWorldMapName = mapName;
                byte[] data = new byte[mapSizeInBytes];
                Marshal.Copy(mapPtr, data, 0, mapSizeInBytes);
                s_arWorldMapData = data;
            }
            OnGotARWorldMapFromDisk(success);
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnARWorldMapLoadedDelegate()
        {
            OnARWorldMapLoaded?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action))]
        private static void OnRelocalizationSucceededDelegate()
        {
            OnRelocalizationSucceeded?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(Action<double, IntPtr>))]
        private static void OnARSessionUpdatedFrameDelegate(double timestamp, IntPtr matrixPtr)
        {
            if (OnARSessionUpdatedFrame == null) return;

            float[] matrixData = new float[16];
            Marshal.Copy(matrixPtr, matrixData, 0, 16);
            Matrix4x4 matrix = new();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[i, j] = matrixData[(4 * i) + j];
                }
            }
            OnARSessionUpdatedFrame(timestamp, matrix);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<double, IntPtr>))]
        private static void OnARSessionUpdatedImageAnchorDelegate(double timestamp, IntPtr matrixPtr)
        {
            if (OnARSessionUpdatedImageAnchor == null) return;

            float[] matrixData = new float[16];
            Marshal.Copy(matrixPtr, matrixData, 0, 16);
            Matrix4x4 matrix = new();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[i, j] = matrixData[(4 * i) + j];
                }
            }
            OnARSessionUpdatedImageAnchor(timestamp, matrix);
        }

        public static event Action<ThermalState> OnThermalStateChanged;

        public static event Action<CameraTrackingState> OnCameraChangedTrackingState;

        public static event Action<ARWorldMappingStatus> OnARWorldMapStatusChanged;

        public static event Action OnGotCurrentARWorldMap;

        public static event Action<string, int> OnCurrentARWorldMapSaved;

        public static event Action<bool> OnGotARWorldMapFromDisk;

        public static event Action OnARWorldMapLoaded;

        public static event Action OnRelocalizationStarted;

        public static event Action OnRelocalizationSucceeded;

        public static event Action<double, Matrix4x4> OnARSessionUpdatedFrame;

        public static event Action<double, Matrix4x4> OnARSessionUpdatedImageAnchor;

        private static XRSessionSubsystem GetLoadedXRSessionSubsystem()
        {
            List<XRSessionSubsystem> xrSessionSubsystems = new();
            SubsystemManager.GetSubsystems(xrSessionSubsystems);
            foreach (var subsystem in xrSessionSubsystems)
            {
                return subsystem;
            }
            Debug.Log("[HoloKitSDK] Failed to get loaded xr session subsystem");
            return null;
        }

        // This function can only be called once per ARSession.
        // Errors will occur if this function is called improperly. Be very careful on this.
        public static void InterceptUnityARSessionDelegate()
        {
            var xrSessionSubsystem = GetLoadedXRSessionSubsystem();
            if (xrSessionSubsystem != null)
            {
                HoloKitSDK_InterceptUnityARSessionDelegate(xrSessionSubsystem.nativePtr);
                Debug.Log("[HoloKitARSessionControllerAPI] Unity ARSession intercepted");
            }
        }

        public static ThermalState GetThermalState()
        {
            return (ThermalState)HoloKitSDK_GetThermalState();
        }

        public static void PauseCurrentARSession()
        {
            HoloKitSDK_PauseCurrentARSession();
        }

        public static void ResumeCurrentARSession()
        {
            HoloKitSDK_ResumeCurrentARSession();
        }

        public static void SetSessionShouldAttemptRelocalization(bool value)
        {
            HoloKitSDK_SetSessionShouldAttemptRelocalization(value);
        }

        public static void SetScaningEnvironment(bool value)
        {
            HoloKitSDK_SetScaningEnvironment(value);
        }

        public static void GetCurrentARWorldMap()
        {
            HoloKitSDK_GetCurrentARWorldMap();
        }

        public static void SaveCurrentARWorldMapWithName(string mapName)
        {
            HoloKitSDK_SaveCurrentARWorldMapWithName(mapName);
        }

        public static void GetARWorldMapFromDiskWithName(string mapName, int mapSizeInBytes)
        {
            HoloKitSDK_GetARWorldMapFromDiskWithName(mapName, mapSizeInBytes);
        }

        public static void NullifyCurrentARWorldMap()
        {
            HoloKitSDK_NullifyCurrentARWorldMap();
            s_arWorldMapData = null;
            s_arWorldMapName = null;
        }

        public static void LoadARWorldMapWithData(byte[] mapData)
        {
            HoloKitSDK_LoadARWorldMapWithData(mapData, mapData.Length);
        }

        public static void RelocalizeToLoadedARWorldMap()
        {
            OnRelocalizationStarted?.Invoke();
            HoloKitSDK_RelocalizeToLoadedARWorldMap();
        }

        public static void SetVideoEnhancementMode(VideoEnhancementMode mode)
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitSDK_SetVideoEnhancementMode((int)mode);
            }
        }

        public static void RegisterARSessionControllerDelegates()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitSDK_RegisterARSessionControllerDelegates(
                    OnThermalStateChangedDelegate,
                    OnCameraChangedTrackingStateDelegate,
                    OnARWorldMapStatusChangedDelegate,
                    OnGotCurrentARWorldMapDelegate,
                    OnCurrentARWorldMapSavedDelegate,
                    OnGotARWorldMapFromDiskDelegate,
                    OnARWorldMapLoadedDelegate,
                    OnRelocalizationSucceededDelegate,
                    OnARSessionUpdatedFrameDelegate,
                    OnARSessionUpdatedImageAnchorDelegate);
            }
        }

        public static List<ARWorldMapDescription> GetARWorldMapListFromDisk()
        {
            string folderPath = Application.persistentDataPath + "/ARWorldMaps";
            if (Directory.Exists(folderPath))
            {
                string[] filePaths = Directory.GetFiles(folderPath);
                if (filePaths.Length == 0)
                {
                    Debug.Log($"[ARSessionController] There is no stored ARWorldMap on disk");
                    return null;
                }
                List<ARWorldMapDescription> maps = new();
                foreach (var filePath in filePaths)
                {
                    string fileName = GetFileNameWithoutSuffix(filePath);
                    FileInfo fileInfo = new(filePath);
                    int mapSizeInBytes = (int)fileInfo.Length;
                    ARWorldMapDescription mapDescription = new()
                    {
                        MapName = fileName,
                        MapFilePath = filePath,
                        MapSizeInBytes = mapSizeInBytes
                    };
                    maps.Add(mapDescription);
                }
                return maps;
            }
            else
            {
                Debug.Log($"[ARSessionController] ARWorldMap folder {folderPath} does not exist");
                return null;
            }
        }

        private static string GetFileNameWithoutSuffix(string filePath)
        {
            string[] strs = filePath.Split("/");
            string fileNameWithSuffix = strs[^1];
            strs = fileNameWithSuffix.Split(".");
            return strs[0];
        }

        public static void DeleteAllARWorldMapsOnDisk()
        {
            string folderPath = Application.persistentDataPath + "/ARWorldMaps";
            if (Directory.Exists(folderPath))
            {
                string[] filePaths = Directory.GetFiles(folderPath);
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
            else
            {
                Debug.Log($"[ARSessionController] ARWorldMap folder {folderPath} does not exist");
            }
        }

        public static void ResetOrigin(Vector3 position, Quaternion rotation)
        {
            float[] p = { position.x, position.y, position.z };
            float[] r = { rotation.x, rotation.y, rotation.z, rotation.w };
            HoloKitSDK_ResetOrigin(p, r);
        }

        public static double GetSystemUptime()
        {
            if (HoloKitUtils.IsRuntime)
            {
                return HoloKitSDK_GetSystemUptime();
            }
            else
            {
                return 0;
            }
        }

        public static void ResetARSessionFirstFrame()
        {
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitSDK_ResetARSessionFirstFrame();
            }
        }

        public static double GetScreenBrightness()
        {
            return HoloKitSDK_GetScreenBrightness();
        }

        public static void SetScreenBrightness(double value)
        {
            if (HoloKitUtils.IsEditor) { return; }
            if (value > 1f || value < 0f)
            {
                Debug.Log("Screen brightness value can only be within 0 and 1");
            }
            HoloKitSDK_SetScreenBrightness(value);
        }
    }
}
