// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollection")]
    public class MetaObjectCollection : NonFungibleCollection
    {
        public override List<NonFungible> NonFungibles
        {
            get
            {
                return MetaObjects.Cast<NonFungible>().ToList();
            }
        }

        public List<MetaObject> MetaObjects;

        public override NonFungible CoverNonFungible => CoverMetaObject;

        public MetaObject CoverMetaObject;

        public override List<Tag> Tags
        {
            get
            {
                return MetaObjectTags.Cast<Tag>().ToList();
            }
        }

        public List<MetaObjectTag> MetaObjectTags;

        public MetaObject GetMetaObject(string tokenId)
        {
            foreach (var metaObject in MetaObjects)
            {
                if (metaObject.TokenId.Equals(tokenId))
                {
                    return metaObject;
                }
            }
            return null;
        }
    }
}