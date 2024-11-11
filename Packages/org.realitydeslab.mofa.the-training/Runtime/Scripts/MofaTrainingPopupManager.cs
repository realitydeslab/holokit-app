// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using RealityDesignLab.MOFA.Library.Base;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.TheTraining
{
    public class MofaTrainingPopupManager : MofaPopupManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private GameObject _findPlanePrefab;

        [SerializeField] private GameObject _placeAvatarPrefab;

        protected override void Start()
        {
            base.Start();

            if (HoloKitApp.Instance.IsHost)
            {
                SpawnPopup(_findPlanePrefab);
            }
        }

        public void OnFoundPlane()
        {
            SpawnPopup(_placeAvatarPrefab);
        }

        public void OnLostPlane()
        {
            SpawnPopup(_findPlanePrefab);
        }
    }
}
