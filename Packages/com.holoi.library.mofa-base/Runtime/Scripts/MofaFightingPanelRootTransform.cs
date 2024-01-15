// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using HoloKit;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelRootTransform : MonoBehaviour
    {
        [SerializeField] private Transform _monoCam;

        [SerializeField] private Transform _centerEye;

        private void Start()
        {
            transform.SetParent(_monoCam);
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                transform.SetParent(_centerEye);
            }
            else
            {
                transform.SetParent(_monoCam);
            }
        }
    }
}
