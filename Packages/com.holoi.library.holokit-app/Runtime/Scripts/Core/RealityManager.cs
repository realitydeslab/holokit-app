using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public struct RealitySessionData
    {
        public string RealityBundleId;

        public float SessionDuration;

        public int PlayerCount;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RealityConfiguration))]
    public abstract class RealityManager : NetworkBehaviour
    {
        [Header("Basics")]
        public GameObject PlayerPrefab;

        public List<GameObject> NetworkPrefabs;

        public List<UI.HoloKitAppUIPanel> UIPanelPrefabs;

        public List<UI.HoloKitAppUIRealitySettingTab> UIRealitySettingTabs;

        [SerializeField] private UniversalRenderPipelineAsset _urpAsset;

        public void SetupURPAsset()
        {
            if (_urpAsset != null)
            {
                GraphicsSettings.renderPipelineAsset = _urpAsset;
            }
        }

        public RealitySessionData GetRealitySessionData()
        {
            RealitySessionData realitySessionData = new()
            {
                RealityBundleId = HoloKitApp.Instance.CurrentReality.BundleId,
                SessionDuration = Time.time - HoloKitCamera.Instance.ARSessionStartTime,
                PlayerCount = HoloKitApp.Instance.MultiplayerManager.PlayerCount
            };
            return realitySessionData;
        }
    }
}