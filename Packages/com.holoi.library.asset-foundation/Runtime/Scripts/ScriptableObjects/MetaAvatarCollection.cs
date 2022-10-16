using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatarCollection")]
    public class MetaAvatarCollection : ArtifactCollection
    {
        public override List<Artifact> Artifacts
        {
            get
            {
                return MetaAvatars.Cast<Artifact>().ToList();
            }
        }

        public List<MetaAvatar> MetaAvatars;

        public override Artifact CoverArtifact => CoverMetaAvatar;

        public MetaAvatar CoverMetaAvatar;

        public override List<Tag> Tags
        {
            get
            {
                return MetaAvatarTags.Cast<Tag>().ToList();
            }
        }

        public List<MetaAvatarTag> MetaAvatarTags;
    }
}