using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatarCollection")]
    public class MetaAvatarCollection : NonFungibleCollection
    {
        public override List<NonFungible> NonFungibles
        {
            get
            {
                return MetaAvatars.Cast<NonFungible>().ToList();
            }
        }

        public List<MetaAvatar> MetaAvatars;

        public override NonFungible CoverNonFungible => CoverMetaAvatar;

        public MetaAvatar CoverMetaAvatar;

        public override List<Tag> Tags
        {
            get
            {
                return MetaAvatarTags.Cast<Tag>().ToList();
            }
        }

        public List<MetaAvatarTag> MetaAvatarTags;

        public MetaAvatar GetMetaAvatar(string tokenId)
        {
            foreach (var metaAvatar in MetaAvatars)
            {
                if (metaAvatar.TokenId.Equals(tokenId))
                {
                    return metaAvatar;
                }
            }
            return null;
        }
    }
}