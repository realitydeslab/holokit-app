// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitAppLib
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
