using UnityEngine;
using Holoi.AssetFoundation;
using UnityEngine.Video;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoulboundToken")]
    public class SoulboundToken : NonFungible 
    {
        public override NonFungibleCollection NonFungibleCollection => SBTCollection;

        public SoulboundTokenCollection SBTCollection;
    }
}