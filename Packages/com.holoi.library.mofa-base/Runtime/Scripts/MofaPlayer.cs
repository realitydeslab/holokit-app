using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
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
        [HideInInspector] public NetworkVariable<MofaTeam> Team = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> KillCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<int> DeathCount = new(0, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone);

        [HideInInspector] public LifeShield LifeShield;

        [HideInInspector] public Vector3 LifeShieldOffest = new(0f, -0.4f, 0.5f);

        public static event Action OnScoreChanged;

        protected virtual void Awake()
        {
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        public override void OnDestroy()
        {
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"[MofaPlayer] OnNetworkSpawned with ownership {OwnerClientId} and team {Team.Value}");

            var mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetPlayer(OwnerClientId, this);

            KillCount.OnValueChanged += OnScoreChangedFunc;
        }

        public override void OnNetworkDespawn()
        {
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