// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
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
