// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFA.TheTraining
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
