// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Apple.PHASE;
using HoloKit;

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
