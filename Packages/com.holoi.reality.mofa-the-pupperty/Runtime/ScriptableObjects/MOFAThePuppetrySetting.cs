using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFAThePuppetry
{

    [CreateAssetMenu(menuName = "ScriptableObjects/Reality/MOFAThePuppetrySetting")]
    public class MOFAThePuppetrySetting : ScriptableObject
    {
        public List<Emote> emotes;
        public MetaAvatar Avatar;
    }
}