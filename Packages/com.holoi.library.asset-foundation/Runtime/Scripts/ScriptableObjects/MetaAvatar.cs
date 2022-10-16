using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatar")]
    public class MetaAvatar : Artifact
    {
        public bool Rigged;

        public Avatar UnityAvatar;

        public override ArtifactCollection ArtifactCollection => MetaAvatarCollection;

        public MetaAvatarCollection MetaAvatarCollection;
    }
}