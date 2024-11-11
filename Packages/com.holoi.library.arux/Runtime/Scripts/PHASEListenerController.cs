// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

#if APPLE_PHASE_ENABLED
using UnityEngine;
using Apple.PHASE;
using HoloKit;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.ARUX
{
    public class PHASEListenerController : MonoBehaviour
    {
        [SerializeField] private PHASEListener _phaseListener;

        [SerializeField] private AudioListener _unityListener;

        private void Awake()
        {
            if (HoloKitUtils.IsRuntime
                && HoloKitAppLib.HoloKitApp.Instance.GlobalSettings.PhaseEnabled
                && HoloKitAppLib.HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
            {
                _phaseListener.enabled = true;
                _unityListener.enabled = false;
            }
            else
            {
                _phaseListener.enabled = false;
                _unityListener.enabled = true;
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_phaseListener.enabled)
            {
                Apple.PHASE.Helpers.PHASEStop();
            }
        }
    }
}
#endif