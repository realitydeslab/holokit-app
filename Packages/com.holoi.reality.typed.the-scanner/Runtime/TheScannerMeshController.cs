// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Reality.Typed.TheScanner
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
