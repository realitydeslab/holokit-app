using System;
using UnityEngine;
using Unity.Netcode;
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
        #endregion
    }
}
