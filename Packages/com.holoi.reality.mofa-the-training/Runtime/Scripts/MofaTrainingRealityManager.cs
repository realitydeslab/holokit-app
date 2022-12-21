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
        [SerializeField] private MofaAIPlayer _mofaAIPlayerPrefab;

        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementManager;

        [SerializeField] private MofaARPlacementIndicator _arPlacementIndicator;

        private MofaAIPlayer _mofaAIPlayer;

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
                SpawnMofaAIPlayer();
        }

        private void SpawnMofaAIPlayer()
        {
            _mofaAIPlayer = Instantiate(_mofaAIPlayerPrefab);
            // TODO: This way of initialization is only temporary
            //Initialize NetworkVariables for HoloKitAppPlayer
            _mofaAIPlayer.PlayerName.Value = "AI";
            _mofaAIPlayer.PlayerType.Value = HoloKitAppPlayerType.Player;
            _mofaAIPlayer.PlayerStatus.Value = HoloKitAppPlayerStatus.Checked;
            // Initialize NetworkVariables for MofaPlayer
            _mofaAIPlayer.Team.Value = MofaTeam.Red;
            _mofaAIPlayer.MagicSchoolIndex.Value = 0;
            _mofaAIPlayer.Ready.Value = true;
            _mofaAIPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(MofaAIPlayer.AIClientId);
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
            else
            {
                GetReady();
            }
        }

        protected override void SetupRound()
        {
            if (RoundCount.Value == 1)
            {
                // Spawn the avatar
                Vector3 position = _arPlacementManager.HitPoint.position;
                Quaternion rotation = _arPlacementManager.HitPoint.rotation;
                var realityBundleId = HoloKitApp.Instance.CurrentReality.BundleId;
                var realityPreferences = HoloKitApp.Instance.GlobalSettings.RealityPreferences[realityBundleId];
                _mofaAIPlayer.SpawnAvatarClientRpc(realityPreferences.MetaAvatarCollectionBundleId, realityPreferences.MetaAvatarTokenId, position, rotation);
                // Turn off ARPlacementManager, ARPlaneManager and ARRaycastManager
                _arPlacementManager.OnPlacedFunc();
                _arPlacementManager.enabled = false;
                _arRaycastManager.enabled = false;
            }

            base.SetupRound();
        }
    }
}
