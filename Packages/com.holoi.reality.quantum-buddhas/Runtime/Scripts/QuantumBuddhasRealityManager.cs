using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.ARUX;

namespace Holoi.Reality.QuantumBuddhas
{
    public class QuantumBuddhasRealityManager : RealityManager
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

        }

        private void Awake()
        {
        }

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.Instance.Active = true;
                HandObject.Instance.enabled = true;
                ARRayCastController.Instance.enabled = true;
            }
            else
            {
                HoloKitHandTracker.Instance.Active = false;
                HandObject.Instance.enabled = false;
                ARRayCastController.Instance.enabled = false;
            }
        }

        private void FixedUpdate()
        {

        }

        [ClientRpc]
        public void OnDisableARRaycastClientRpc()
        {
            Debug.Log($"OnDisableARRaycastClientRpc");
            GetComponent<QuantumBuddhasSceneManager>().DisableARRaycastClientRpc();

        }

        [ClientRpc]
        public void OnActiveBuddhasSwitchClientRpc(int index)
        {
            Debug.Log($"OnActiveBuddhasChangedClientRpc: {index}");
            GetComponent<QuantumBuddhasSceneManager>().SwitchToNextVFXClientRpc();

        }

        [ClientRpc]
        public void OnBuddhasEnabledClientRpc()
        {
            Debug.Log($"OnBuddhasEnabledClientRpc");

            GetComponent<QuantumBuddhasSceneManager>().InitTargetGameObjectClient();
        }
    }
}