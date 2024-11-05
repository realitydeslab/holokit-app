// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp;

namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class BlankPiecesOfPaperManager : RealityManager
    {
        [SerializeField] NetworkObject SinglePagePrefab;
        NetworkObject _singlePageInstance;
        Transform _centereye;

        void Start()
        {
            _centereye = HoloKit.HoloKitCameraManager.Instance.CenterEyePose;
        }

        public void CreateSinglePage()
        {
            Debug.Log("CreateSinglePage");
            _singlePageInstance = Instantiate(SinglePagePrefab);
            _singlePageInstance.transform.position = _centereye.position + _centereye.forward * 2f;
            _singlePageInstance.Spawn();
        }
    }
}
