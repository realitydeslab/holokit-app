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
                SpawnMofaPlayer(MofaTeam.Blue, NetworkManager.LocalClientId);

                // Spawn AI's player
                SpawnMofaPlayer(MofaTeam.Red, 999);

                // Spawn host's life shield
                SpawnLifeShield(NetworkManager.LocalClientId);
            }
        }
    }
}
