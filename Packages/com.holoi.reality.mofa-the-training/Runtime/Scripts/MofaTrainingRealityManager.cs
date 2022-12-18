using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private MofaPlayerAI _mofaPlayerAIPrefab;

        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementIndicator _arPlacementIndicator;

        [SerializeField] private MofaARPlacementIndicatorVfxController _arPlacementIndicatorVfxController;

        private MofaPlayerAI _mofaPlayerAI;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                _arPlacementIndicator.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_arPlacementIndicatorVfxController.gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                SpawnMofaPlayerAI();
            }
        }

        private void SpawnMofaPlayerAI()
        {
            _mofaPlayerAI = Instantiate(_mofaPlayerAIPrefab);
            // TODO: This is hard-coded
            _mofaPlayerAI.MagicSchool.Value = 0;
            _mofaPlayerAI.Team.Value = MofaTeam.Red;
            _mofaPlayerAI.GetComponent<NetworkObject>().SpawnWithOwnership(MofaPlayerAI.AIClientId);
        }

        public override void TryStartRound()
        {
            //if (CurrentPhase != MofaPhase.Waiting && CurrentPhase != MofaPhase.RoundData)
            //{
            //    Debug.Log($"[MofaTrainingRealityManager] You cannot start round at the current phase: {CurrentPhase}");
            //    return;
            //}

            //if (RoundCount == 0)
            //{
            //    if (_arPlacementIndicator != null && _arPlacementIndicator.IsActive && _arPlacementIndicator.IsValid)
            //    {
            //        Vector3 position = _arPlacementIndicator.HitPoint.position;
            //        Quaternion rotation = _arPlacementIndicator.HitPoint.rotation;
            //        _arPlaneManager.enabled = false;
            //        _arRaycastManager.enabled = false;
            //        _arPlacementIndicator.OnPlacedFunc();
            //        var realityPreferences = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId];
            //        _mofaPlayerAI.InitializeAvatarClientRpc(position,
            //                                                rotation,
            //                                                realityPreferences.MetaAvatarCollectionBundleId,
            //                                                realityPreferences.MetaAvatarTokenId);
            //        StartCoroutine(StartBaseRoundFlow());
            //    }
            //    else
            //    {
            //        Debug.Log("[MOFATheTraining] Failed to start round");
            //    }
            //}
            //else
            //{
            //    StartCoroutine(StartBaseRoundFlow());
            //}
        }
    }
}
