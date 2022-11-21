using UnityEngine;
using Unity.Netcode;
using HoloKit;
using System;

namespace Holoi.Library.MOFABase
{
    public enum MofaTeam
    {
        Blue = 0,
        Red = 1
    }

    public class MofaPlayer : NetworkBehaviour
    {
        [HideInInspector] public NetworkVariable<int> MagicSchoolTokenId = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public NetworkVariable<MofaTeam> Team = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public NetworkVariable<int> KillCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public NetworkVariable<int> DeathCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player cast spells in this round?
        /// </summary>
        [HideInInspector] public NetworkVariable<int> CastCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player hit in this round?
        /// </summary>
        [HideInInspector] public NetworkVariable<int> HitCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public LifeShield LifeShield;

        [HideInInspector] public Vector3 LifeShieldOffset = new(0f, -0.4f, 0.5f);

        public static event Action<MofaPlayer> OnMofaPlayerSpawned;

        public static event Action<ulong, bool> OnMofaPlayerReadyStateChanged;

        public static event Action OnScoreChanged;

        protected virtual void Start()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        public override void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        public override void OnNetworkSpawn()
        {
            OnMofaPlayerSpawned?.Invoke(this);
            var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaBaseRealityManager.SetPlayer(OwnerClientId, this);
            mofaBaseRealityManager.SpellPool.OnPlayerJoined(MagicSchoolTokenId.Value);

            Ready.OnValueChanged += OnReadyStateChangedFunc;
            KillCount.OnValueChanged += OnScoreChangedFunc;
        }

        public override void OnNetworkDespawn()
        {
            Ready.OnValueChanged -= OnReadyStateChangedFunc;
            KillCount.OnValueChanged -= OnScoreChangedFunc;
        }

        protected virtual void FixedUpdate()
        {
            // Update ClientNetworkTransform
            if (IsOwner)
            {
                transform.SetPositionAndRotation(HoloKitCamera.Instance.CenterEyePose.position, HoloKitCamera.Instance.CenterEyePose.rotation);
            }
        }

        private void OnScoreChangedFunc(int oldValue, int newValue)
        {
            OnScoreChanged?.Invoke();
        }

        private void OnReadyStateChangedFunc(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                OnMofaPlayerReadyStateChanged?.Invoke(OwnerClientId, newValue);
                if (IsServer)
                {

                    ((MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager).OnPlayerReadyStateChanged();
                }
            }
        }

        protected virtual void OnPhaseChanged(MofaPhase mofaPhase)
        {
            if (IsServer)
            {
                if (mofaPhase == MofaPhase.Countdown)
                {
                    KillCount.Value = 0;
                    DeathCount.Value = 0;
                }
            }
        }
    }
}