// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using HoloKit;
using RealityDesignLab.Library.ARUX;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.Typed.TheForce
{
    public class TypedTheForceRealityManager : RealityManager
    {
        Transform _centereye;
        [SerializeField] Transform _handJoint;
        [SerializeField] Player _player;
        [SerializeField]GameObject _prefabMC;
        [SerializeField]GameObject _prefabMO;

        int n = 0;
        int m = 0;
        float _lastTriggerTime = 4;

        private void Start()
        {
            _centereye = HoloKitCameraManager.Instance.CenterEyePose;
            StartCoroutine(WaitAndCreateMagicCube());
        }

        void SceneSetup()
        {
            LayerMask lm = 1 << 7;
            Vector3 dir = new Vector3(Random.Range(-1f,1f), -1 , Random.Range(0f,1f));
            Ray ray = new Ray(_centereye.position, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3.0f, lm))
            {
                Debug.Log("hit ar sroundings");
                CreateMagicObjct(hit.point);
                n++;
            }
        }

        void CreateMagicCube()
        {
            var go = Instantiate(_prefabMC);
            go.transform.position = _centereye.position + _centereye.forward;
            go.transform.GetComponent<FollowMovementManager>().FollowTarget = _handJoint;
        }
        void CreateMagicObjct(Vector3 position)
        {
            var go = Instantiate(_prefabMO);
            go.transform.position = position + Vector3.up*0.5f;
        }

        private void Update()
        {
            //if (m == 0)
            //{
            //    CreateMagicCube();
            //    m++;
            //}

            if (n == 0)
            {
                if (Time.time - _lastTriggerTime > 4f)
                {
                    SceneSetup();
                    _lastTriggerTime = Time.time;
                }
            }
        }

        IEnumerator WaitAndCreateMagicCube()
        {
            yield return new WaitForSeconds(4f);
            CreateMagicCube();
        }
    }
}
