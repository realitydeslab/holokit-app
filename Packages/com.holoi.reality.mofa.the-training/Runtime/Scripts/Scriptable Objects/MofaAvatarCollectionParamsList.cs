// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
