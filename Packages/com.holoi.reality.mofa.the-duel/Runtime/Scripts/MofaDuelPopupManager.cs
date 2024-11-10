// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Linq;
using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFA.TheDuel
{
    public class MofaDuelPopupManager : MofaPopupManager
    {
        [Header("MOFA The Duel")]
        [SerializeField] private GameObject _getReadyPrefab;

        [SerializeField] private GameObject _waitingOthersPrefab;

        protected override void Start()
        {
            base.Start();
            if (HoloKitApp.Instance.IsPlayer)
            {
                SpawnPopup(_getReadyPrefab);
                MofaPlayer.OnMofaPlayerReadyChanged += OnMofaPlayerReadyChanged;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsPlayer)
            {
                MofaPlayer.OnMofaPlayerReadyChanged -= OnMofaPlayerReadyChanged;
            }
        }

        private void OnMofaPlayerReadyChanged(MofaPlayer mofaPlayer)
        {
            if (mofaPlayer.IsLocalPlayer && mofaPlayer.Ready.Value)
            {
                var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                var mofaPlayerList = mofaBaseRealityManager.MofaPlayerList;
                if (mofaPlayerList.Any(t => t.Ready.Value == false))
                    SpawnPopup(_waitingOthersPrefab);
            }  
        }
    }
}
