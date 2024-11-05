// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;
using System;

namespace RealityDesignLab.Library.HoloKitApp
{
    public class CoachingOverlaySessionDelegate : DefaultARKitSessionDelegate
    {
        public static event Action OnCoachingOverlayViewStarted;

        public static event Action OnCoachingOverlayViewEnded;

        protected override void OnCoachingOverlayViewWillActivate(ARKitSessionSubsystem sessionSubsystem)
        {
            OnCoachingOverlayViewStarted?.Invoke();
        }

        protected override void OnCoachingOverlayViewDidDeactivate(ARKitSessionSubsystem sessionSubsystem)
        {
            OnCoachingOverlayViewEnded?.Invoke();
        }
    }
}
