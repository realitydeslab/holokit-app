// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode.Components;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.QuantumRealm
{
    public class QuantumRealmHostHandPoint : HostHandPoint
    {
        [SerializeField] private HoverObject _hoverObject;

        [SerializeField] private NetworkTransform _networkTransform;

        [SerializeField] private GameObject _hostHandPointVisual;

        private MeshRenderer _hosthandPointVisualMeshRenderer;

        private const float HostHandPointVisualReappearTime = 1f;

        private const float HostHandPointVisualDisappearTime = 0.5f;

        protected override void Start()
        {
            base.Start();
            _hosthandPointVisualMeshRenderer = _hostHandPointVisual.GetComponent<MeshRenderer>();
            if (HoloKitUtils.IsRuntime)
            {
                OnHostHandValidityChanged(false);
            }
            else
            {
                OnHostHandValidityChanged(true);
            }
        }

        protected override void OnHostHandValidityChanged(bool isValid)
        {
            if (isValid)
            {
                // Make it reappear
                _networkTransform.Interpolate = true;
                LeanTween.value(0f, 0.8f, HostHandPointVisualReappearTime)
                    .setOnUpdate((float alpha) => {
                        _hosthandPointVisualMeshRenderer.material.SetFloat("_Alpha", alpha);
                    })
                    .setOnComplete(() => {
                        _hoverObject.IsActive = true;
                    });
            }
            else
            {
                // Make it disappear
                LeanTween.value(0.8f, 0f, HostHandPointVisualDisappearTime)
                    .setOnUpdate((float alpha) => {
                        _hosthandPointVisualMeshRenderer.material.SetFloat("_Alpha", alpha);
                    })
                    .setOnComplete(() => {
                        _hoverObject.IsActive = false;
                        _networkTransform.Interpolate = false;
                    });
            }
        }
    }
}
