// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.Typed.TheScanner
{
    public class TheScannerMeshController : MonoBehaviour
    {
        private Material _material;

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if (TheScannerCenter.Instance != null)
            {
                _material.SetVector("_Center_Position", TheScannerCenter.Instance.transform.position);
                _material.SetFloat("_Angle_Offset", -TheScannerCenter.Instance.transform.rotation.eulerAngles.y);
            }
        }
    }
}
