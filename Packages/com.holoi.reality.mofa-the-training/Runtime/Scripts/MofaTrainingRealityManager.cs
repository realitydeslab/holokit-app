using System;
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
        [SerializeField] private MofaAIPlayer _mofaPlayerAIPrefab;

        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementManager;

        [SerializeField] private MofaARPlacementIndicator _arPlacementIndicator;

        private MofaAIPlayer _mofaPlayerAI;

        /// <summary>
        /// This event is called when the player tries to start the game in an invalid position.
        /// </summary>
        public static event Action OnFailedToStartAtCurrentPosition;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                _arPlacementManager.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementManager.gameObject);
                Destroy(_arPlacementIndicator.gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                //SpawnMofaPlayerAI();
            }
        }

        private void SpawnMofaPlayerAI()
        {
            _mofaPlayerAI = Instantiate(_mofaPlayerAIPrefab);
            _mofaPlayerAI.GetComponent<NetworkObject>().SpawnWithOwnership(MofaAIPlayer.AIPlayerClientId);
            _mofaPlayerAI.MagicSchoolIndex.Value = 0;
            _mofaPlayerAI.Team.Value = MofaTeam.Red;
        }

        public override void TryGetReady()
        {
            // We only need to select a proper spawn point in the first round
            if (RoundCount.Value == 1)
            {
                if (_arPlacementManager.IsValid)
                    GetReady();
                else
                    OnFailedToStartAtCurrentPosition?.Invoke();
            }
        }

        protected override void SetupRound()
        {
            base.SetupRound();

        }

        protected override void StartRound()
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
