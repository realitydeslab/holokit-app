using System;
using UnityEngine;
using Unity.Netcode;
using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonController : NetworkBehaviour
    {
        [SerializeField] private MAnimal _animal;

        [SerializeField] private Aim _aim;

        [Header("ModeID")]
        [SerializeField] private ModeID _attack1;

        [SerializeField] private ModeID _attack1Air;

        [SerializeField] private ModeID _attack2;

        [SerializeField] private ModeID _action;

        public static event Action OnDragonSpawned;

        private void Awake()
        {
            _animal.m_MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
        }

        private void Start()
        {
            _aim.MainCamera = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetTheDragonController(this);
            OnDragonSpawned?.Invoke();
        }

        #region Network Callbacks
        [ClientRpc]
        public void Movement_UseCameraInputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.UseCameraInput = value;
            }
        }

        [ClientRpc]
        public void Movement_SetUpDownAxisClientRpc(float value)
        {
            if (!IsOwner)
            {
                _animal.SetUpDownAxis(value);
            }
        }

        [ClientRpc]
        public void Movement_SetInputAxisClientRpc(Vector2 value)
        {
            if (!IsOwner)
            {
                _animal.SetInputAxis(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack1ClientRpc()
        {
            Debug.Log("Mode_Pin_Attack1ClientRpc");
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack1);
            }
        }

        [ClientRpc]
        public void Mode_Pin_InputClientRpc(bool value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Input(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_AbilityClientRpc(int value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Ability(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack1AirClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack1Air);
            }
        }

        [ClientRpc]
        public void Mode_Pin_Attack2ClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_attack2);
            }
        }

        [ClientRpc]
        public void Mode_InterruptClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Interrupt();
            }
        }

        [ClientRpc]
        public void Mode_Pin_ActionClientRpc()
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin(_action);
            }
        }

        [ClientRpc]
        public void Mode_Pin_TimeClientRpc(float value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Time(value);
            }
        }

        [ClientRpc]
        public void Mode_Pin_StatusClientRpc(int value)
        {
            if (!IsOwner)
            {
                _animal.Mode_Pin_Status(value);
            }
        }
        #endregion
    }
}
