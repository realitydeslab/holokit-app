using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaAvatar")]
    public class MetaAvatar : NonFungible
    {
        public bool Rigged;

        public Avatar UnityAvatar;

        public override NonFungibleCollection NonFungibleCollection => MetaAvatarCollection;

        public MetaAvatarCollection MetaAvatarCollection;
    }
}