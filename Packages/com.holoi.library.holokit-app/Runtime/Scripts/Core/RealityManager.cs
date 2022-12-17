using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public abstract class RealityManager : NetworkBehaviour
    {
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
    }
}