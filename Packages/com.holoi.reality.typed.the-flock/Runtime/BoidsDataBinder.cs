// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using BoidsSimulationOnGPU;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;


[AddComponentMenu("VFX/Property Binders/GPUBoids/BoidsData Binder")]
[VFXBinder("GPUBoids/BoidsData")]
sealed class VFXBoidsDataBinder : VFXBinderBase
{
    public string Property
    {
        get => (string) _property;
        set => _property = value;
    }

    [VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
    ExposedProperty _property = "DataSet";

    public GPUBoids Source = null;

    public override bool IsValid(VisualEffect component)
        => Source != null && component.HasGraphicsBuffer(_property);

    public override void UpdateBinding(VisualEffect component)
        => component.SetGraphicsBuffer(_property, Source.GetBoidPositionDataBuffer());

    public override string ToString()
        => $"BoidsData : '{_property}' -> {Source?.name ?? "(null)"}";
}