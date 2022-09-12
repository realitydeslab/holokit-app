using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Mofa.Base;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // Spawn host's player
                var hostPlayer = Instantiate(MofaPlayerPrefab);
                hostPlayer.GetComponent<NetworkObject>().Spawn();

                // Spawn AI's player
                var aiPlayer = Instantiate(MofaPlayerPrefab);
                aiPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(999);
            }
        }
    }
}
