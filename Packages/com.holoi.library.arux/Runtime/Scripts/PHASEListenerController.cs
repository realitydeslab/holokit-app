// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

#if APPLE_PHASE_ENABLED
using UnityEngine;
using Apple.PHASE;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class PHASEListenerController : MonoBehaviour
    {
        [SerializeField] private PHASEListener _phaseListener;

        [SerializeField] private AudioListener _unityListener;

        private void Awake()
        {
            if (HoloKitUtils.IsRuntime
                && HoloKitApp.HoloKitApp.Instance.GlobalSettings.PhaseEnabled
                && HoloKitApp.HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
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