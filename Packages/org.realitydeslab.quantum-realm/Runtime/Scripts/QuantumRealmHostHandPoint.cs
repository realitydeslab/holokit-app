// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode.Components;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.ARUX;
using HoloKit;

namespace RealityDesignLab.QuantumRealm
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
