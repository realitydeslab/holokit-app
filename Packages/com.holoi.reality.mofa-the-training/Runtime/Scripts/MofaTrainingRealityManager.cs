using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.MOFABase;
using Holoi.Library.MOFABase.WatchConnectivity;
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

        protected override void Start()
        {
            base.Start();
            HoloKitAppUIEventManager.OnTriggered += OnTriggered;
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived += OnStartRoundMessageReceived;

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

        public override void OnDestroy()
        {
            base.OnDestroy();

            HoloKitAppUIEventManager.OnTriggered -= OnTriggered;
            MofaWatchConnectivityAPI.OnStartRoundMessageReceived -= OnStartRoundMessageReceived;
        }

        private void SpawnMofaPlayerAI()
        {
            _mofaPlayerAI = Instantiate(_mofaPlayerAIPrefab);
            // TODO: This is hard-coded
            _mofaPlayerAI.MagicSchoolTokenId.Value = 0;
            _mofaPlayerAI.Team.Value = MofaTeam.Red;
            _mofaPlayerAI.GetComponent<NetworkObject>().SpawnWithOwnership(999);
        }

        protected override void StartRound()
        {
            if (CurrentPhase != MofaPhase.Waiting && CurrentPhase != MofaPhase.RoundData)
            {
                Debug.Log($"[MofaBaseRealityManager] You cannot start round at the current phase: {CurrentPhase}");
                return;
            }

            if (RoundCount == 0)
            {
                if (_arPlacementIndicator != null && _arPlacementIndicator.IsActive && _arPlacementIndicator.IsValid)
                {
                    _arPlaneManager.enabled = false;
                    _arRaycastManager.enabled = false;
                    _arPlacementIndicator.OnPlacedFunc();
                    _mofaPlayerAI.InitializeAvatarClientRpc(_arPlacementIndicator.HitPoint.position,
                                                            _arPlacementIndicator.HitPoint.rotation,
                                                            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId].MetaAvatarCollectionBundleId,
                                                            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId].MetaAvatarTokenId);
                    StartCoroutine(StartRoundFlow());
                }
                else
                {
                    Debug.Log("[MOFATheTraining] Failed to start round");
                }
            }
            else
            {
                StartCoroutine(StartRoundFlow());
            }
        }

        private void OnTriggered()
        {
            if (!HoloKitApp.Instance.IsHost) { return; }

            if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
            {
                StartRound();
            }
        }

        // Apple Watch
        private void OnStartRoundMessageReceived()
        {
            StartRound();
        }
    }
}
