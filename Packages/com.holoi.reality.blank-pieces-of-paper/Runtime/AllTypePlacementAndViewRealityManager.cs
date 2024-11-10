// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class AllTypePlacementAndViewRealityManager : RealityManager
    {
        [SerializeField] List<NetworkObject> PaperFlockList;

        NetworkObject _PaperFlockInstance;

        Transform _centereye;

        int currentIndex = 0;


        void Start()
        {
            _centereye = HoloKit.HoloKitCameraManager.Instance.CenterEyePose;
        }

        public void CreatePaperFlock()
        {
            Debug.Log("CreatePaperFlock");

            // destory the last
            if (_PaperFlockInstance != null)
            {
                Destroy(_PaperFlockInstance);
                _PaperFlockInstance.Despawn();
            }


            // create the new
            _PaperFlockInstance = Instantiate(PaperFlockList[currentIndex]);
            _PaperFlockInstance.transform.position = _centereye.position + _centereye.forward * 1f;
            _PaperFlockInstance.Spawn();

            currentIndex++;
            if (currentIndex > PaperFlockList.Count - 1) currentIndex = 0;
        }
    }
}
