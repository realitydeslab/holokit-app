using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.Typography
{
    public class TypoSpaceRealityManager : RealityManager
    {
        [SerializeField] private ARMeshManager _arMeshManager;

        private void Start()
        {
            if (HoloKitApp.Instance.IsSpectator)
            {
                HoloKitAppMultiplayerManager.OnAlignmentMarkerChecked += OnTurnOnMeshing;
                HoloKitAppUIPanel_MonoAR_RescanQRCode.OnRescanQRCode += OnTurnOffMeshing;
                HoloKitAppUIPanel_MonoAR_RescanQRCode.OnCancelRescanQRCode += OnTurnOnMeshing;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _arMeshManager.enabled = true;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsSpectator)
            {
                HoloKitAppMultiplayerManager.OnAlignmentMarkerChecked -= OnTurnOnMeshing;
                HoloKitAppUIPanel_MonoAR_RescanQRCode.OnRescanQRCode -= OnTurnOffMeshing;
                HoloKitAppUIPanel_MonoAR_RescanQRCode.OnCancelRescanQRCode -= OnTurnOnMeshing;
            }
        }

        private void OnTurnOnMeshing()
        {
            _arMeshManager.enabled = true;
        }

        private void OnTurnOffMeshing()
        {
            _arMeshManager.enabled = false;
        }
    }
}
