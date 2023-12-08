// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp
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
