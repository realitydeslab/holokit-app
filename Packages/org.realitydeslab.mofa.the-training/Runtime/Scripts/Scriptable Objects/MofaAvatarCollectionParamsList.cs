// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace RealityDesignLab.MOFA.TheTraining
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAvatarCollectionParamsList")]
    public class MofaAvatarCollectionParamsList : ScriptableObject
    {
        public List<MofaAvatarCollectionParams> List;

        public MofaAvatarCollectionParams GetAvatarCollectionParams(MetaAvatarCollection avatarCollection)
        {
            foreach (var avatarCollectionParams in List)
            {
                if (avatarCollectionParams.MetaAvatarCollection.Equals(avatarCollection))
                {
                    return avatarCollectionParams;
                }
            }
            return null;
        }
    }
}
