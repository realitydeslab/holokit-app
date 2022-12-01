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

        [HideInInspector] public NetworkVariable<int> Kill = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public NetworkVariable<int> Death = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player cast spells in this round?
        /// </summary>
        [HideInInspector] public NetworkVariable<int> CastCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player hit in this round?
        /// </summary>
        [HideInInspector] public NetworkVariable<int> HitCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The distance the player has moved in this round.
        /// </summary>
        [HideInInspector] public NetworkVariable<float> Distance = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public NetworkVariable<float> Calories = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [HideInInspector] public LifeShield LifeShield;

        [HideInInspector] public Vector3 LifeShieldOffset = new(0f, -0.4f, 0.5f);

        private bool _isFighting;

        /// <summary>
        /// We update this value during the fight to reduce network throughput.
        /// </summary>
        private float _distance;

        private Vector3 _lastFramePosition;

        public static event Action<MofaPlayer> OnMofaPlayerSpawned;

        public static event Action<ulong, bool> OnMofaPlayerReadyStateChanged;

        public static event Action OnScoreChanged;

        public static event Action OnHealthDataUpdated;

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
            Kill.OnValueChanged += OnScoreChangedFunc;
            Distance.OnValueChanged += OnDistanceChangedFunc;
            Calories.OnValueChanged += OnCaloriesChangedFunc;
        }

        public override void OnNetworkDespawn()
        {
            Ready.OnValueChanged -= OnReadyStateChangedFunc;
            Kill.OnValueChanged -= OnScoreChangedFunc;
            Distance.OnValueChanged -= OnDistanceChangedFunc;
            Calories.OnValueChanged -= OnCaloriesChangedFunc;
        }

        protected virtual void Update()
        {
            if (IsServer)
            {
                if (_isFighting)
                {
                    // We only compute horizontal distance
                    Vector3 position = Vector3.ProjectOnPlane(transform.position, Vector3.up);
                    _distance += Vector3.Distance(_lastFramePosition, position);
                    _lastFramePosition = position;
                }
            }
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

        private void OnDistanceChangedFunc(float oldValue, float newValue)
        {
            if (oldValue != newValue)
            {
                OnHealthDataUpdated?.Invoke();
            }
        }

        private void OnCaloriesChangedFunc(float oldValue, float newValue)
        {
            if (oldValue != newValue)
            {
                OnHealthDataUpdated?.Invoke();
            }
        }

        private void OnReadyStateChangedFunc(bool oldValue, bool newValue)
        {
            // We only react to the situation where the ready state changes
            // from false to true
            if (!oldValue && newValue)
            {
                OnMofaPlayerReadyStateChanged?.Invoke(OwnerClientId, newValue);
            }
        }

        protected virtual void OnPhaseChanged(MofaPhase mofaPhase)
        {
            if (IsServer)
            {
                if (mofaPhase == MofaPhase.Countdown)
                {
                    Kill.Value = 0;
                    Death.Value = 0;
                    CastCount.Value = 0;
                    HitCount.Value = 0;
                    Distance.Value = 0f;
                    Calories.Value = 0f;
                    _distance = 0f;
                }
                else if (mofaPhase == MofaPhase.Fighting)
                {
                    _lastFramePosition = transform.position;
                    _isFighting = true;
                }
                else if (mofaPhase == MofaPhase.RoundOver)
                {
                    _isFighting = false;
                    // We only update this network variable once in each round
                    Distance.Value = Mathf.RoundToInt(_distance);
                }
            }
        }

        [ServerRpc]
        public void UpdateHealthDataServerRpc(float distance, float calories)
        {
            _distance = distance;
            Distance.Value = distance;
            Calories.Value = calories;
        }
    }
}