// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.Typed.TheDuck
{
    public class TypedTheDuckRealityManager : RealityManager
    {
        public GameObject DuckPrefab;

        GameObject duckInstance;

        [HideInInspector] public Vector2 DuckMaxSpeed = new(10,10);

        public void OnDragBegin()
        {
            duckInstance = Instantiate(DuckPrefab);
            duckInstance.transform.position =
                HoloKit.HoloKitCameraManager.Instance.CenterEyePose.position
                + HoloKit.HoloKitCameraManager.Instance.CenterEyePose.forward * 1f
                - HoloKit.HoloKitCameraManager.Instance.CenterEyePose.up * 0.5f;
            duckInstance.GetComponent<Rigidbody>().useGravity = false;
        }

        public void OnDrag()
        {
            //duckInstance.transform.position = position;
        }

        public void OnDragEnd()
        {
            Debug.Log("OnDragEnd!!!!");
            var length = FindObjectOfType<DragableUI>().Length;
            var direction = FindObjectOfType<DragableUI>().Direction;
            //Debug.Log("length: " + length);
            //Debug.Log("direction: " + direction);

            duckInstance.GetComponent<Rigidbody>().useGravity = true;
            var fixedYSpeed = Remap(length, 0, 600, 0, DuckMaxSpeed.y, true);
            var fixedXSpeed = Remap(length, 0, 600, 0, DuckMaxSpeed.x, true);

            duckInstance.GetComponent<Rigidbody>().velocity = -direction * fixedYSpeed + HoloKit.HoloKitCameraManager.Instance.CenterEyePose.forward * fixedXSpeed;
        }

        float Remap(float x, float inMin, float inMax, float outMin, float outMax, bool clamp = false)
        {
            if (clamp)
            {
                if (inMin <= inMax)
                {
                    if (x < inMin) return outMin;
                    if (x > inMax) return outMax;
                }
                else
                {
                    if (x < inMax) return outMin;
                    if (x > inMin) return outMax;
                }

            }

            x = (((x - inMin) / (inMax - inMin)) * (outMax - outMin)) + outMin;
            return x;
        }
    }
}
