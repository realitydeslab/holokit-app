using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;

namespace Holoi.Realities.GoodBuddy
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityGoodBuddySetting")]
    public class RealityGoodBuddySetting : ScriptableObject
    {
        public List<Emote> emotes;
        public MetaAvatar metaAvatar;
    }
}