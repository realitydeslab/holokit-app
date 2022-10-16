using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObjectCollection")]
    public class MetaObjectCollection : ArtifactCollection
    {
        public override List<Artifact> Artifacts
        {
            get
            {
                return MetaObjects.Cast<Artifact>().ToList();
            }
        }

        public List<MetaObject> MetaObjects;

        public override Artifact CoverArtifact => CoverMetaObject;

        public MetaObject CoverMetaObject;

        public override List<Tag> Tags
        {
            get
            {
                return MetaObjectTags.Cast<Tag>().ToList();
            }
        }

        public List<MetaObjectTag> MetaObjectTags;
    }
}