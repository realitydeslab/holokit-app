// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.AssetFoundation;

namespace RealityDesignLab.MOFA.TheTraining
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAvatarCollectionParams")]
    public class MofaAvatarCollectionParams : ScriptableObject
    {
        public MetaAvatarCollection MetaAvatarCollection;

        public float Scale;

        // From avatar center to life shield center
        public Vector3 CenterEyeToLifeShiledOffset;

        // From avatar origin to avatar's center eye
        public Vector3 AvatarOriginToCenterEyeOffset;
    }
}
