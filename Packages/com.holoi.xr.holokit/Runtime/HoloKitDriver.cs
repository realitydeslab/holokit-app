using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace HoloKit
{
    public class HoloKitDriver : MonoBehaviour
    {
        [SerializeField] private string[] _arSceneNames;

        [SerializeField] private bool _sessionShouldAttemptRelocalization = false;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (HoloKitUtils.IsRuntime)
            {
                HoloKitNFCSessionControllerAPI.RegisterNFCSessionControllerDelegates();
                HoloKitARSessionControllerAPI.RegisterARSessionControllerDelegates();
                HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                HoloKitARSessionControllerAPI.SetSessionShouldAttemptRelocalization(_sessionShouldAttemptRelocalization);
                SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            foreach (var arSceneName in _arSceneNames)
            {
                if (scene.name.Equals(arSceneName))
                {
                    LoaderUtility.Deinitialize();
                    LoaderUtility.Initialize();
                    HoloKitARSessionControllerAPI.InterceptUnityARSessionDelegate();
                    return;
                }
            }
        }
    }
}
