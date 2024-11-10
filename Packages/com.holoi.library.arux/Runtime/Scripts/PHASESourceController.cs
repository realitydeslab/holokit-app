// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT


#if APPLE_PHASE_ENABLED
using UnityEngine;
using Apple.PHASE;
using HoloKit;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.ARUX
{
    public class PHASESourceController : MonoBehaviour
    {
        [SerializeField] private PHASESource _phaseSource;

        [SerializeField] private AudioSource _unityAudioSource;

        private void Awake()
        {
            if (HoloKitUtils.IsRuntime
                && HoloKitApp.HoloKitApp.Instance.GlobalSettings.PhaseEnabled
                && HoloKitApp.HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
            {
                _phaseSource.enabled = true;
                _unityAudioSource.enabled = false;
            }
            else
            {
                _phaseSource.enabled = false;
                _unityAudioSource.enabled = true;
            }
        }

        private void OnDestroy()
        {
            if (_phaseSource.enabled && _phaseSource.IsPlaying())
            {
                _phaseSource.Stop();
            }
        }
    }
}
#endif 