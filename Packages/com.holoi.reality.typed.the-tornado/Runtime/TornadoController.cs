// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typed.TheTornado
{
    public class TornadoController : MonoBehaviour
    {
        private void Start()
        {
            ((TypedTheTornadoRealityManager) HoloKitApp.Instance.RealityManager).GroundPosY.OnValueChanged += OnGroundPosYValueChanged;
        }

        private void OnDestroy()
        {
            var realityManager = (TypedTheTornadoRealityManager) HoloKitApp.Instance.RealityManager;
            if (realityManager != null && realityManager.GroundPosY != null)
            {
                realityManager.GroundPosY.OnValueChanged -= OnGroundPosYValueChanged;
            }
        }

        private void OnGroundPosYValueChanged(float oldValue, float newValue)
        {
            transform.position = new(transform.position.x, newValue, transform.position.z);
        }
    }
}
