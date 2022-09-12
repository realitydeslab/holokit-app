using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.HoloKit.App;

namespace Holoi.Mofa.Base
{
    public class MofaBaseRealityManager : RealityManager
    {
        public MofaPlayer MofaPlayerPrefab;

        public Dictionary<ulong, MofaPlayer> Players = new();

        public void SetPlayer(ulong clientId, MofaPlayer mofaPlayer)
        {
            Players[clientId] = mofaPlayer;
            mofaPlayer.transform.SetParent(transform);
        }

        public void StartRound()
        {

        }
    }
}