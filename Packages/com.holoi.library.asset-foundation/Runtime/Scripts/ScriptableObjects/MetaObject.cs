using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObject")]
    public class MetaObject : Artifact
    {
        public override ArtifactCollection ArtifactCollection => MetaObjectCollection;

        public MetaObjectCollection MetaObjectCollection;
    }
}