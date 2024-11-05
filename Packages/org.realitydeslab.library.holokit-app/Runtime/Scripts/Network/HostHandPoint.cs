// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.Library.HoloKitApp
{
    public abstract class HostHandPoint : MonoBehaviour
    {
        [SerializeField] private NetworkHoloKitHandTracker _networkHandTracker;

        protected virtual void Start()
        {
            NetworkHoloKitHandTracker.OnHostHandValidityChanged += OnHostHandValidityChanged;
        }

        protected virtual void OnDestroy()
        {
            NetworkHoloKitHandTracker.OnHostHandValidityChanged -= OnHostHandValidityChanged;
        }

        protected abstract void OnHostHandValidityChanged(bool isValid);
    }
}
