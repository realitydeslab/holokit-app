// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFA.TheTraining
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
