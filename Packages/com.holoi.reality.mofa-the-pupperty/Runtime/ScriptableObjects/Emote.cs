using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFAThePuppetry
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MOFAThePuppetry/Emote")]
    public class Emote: ScriptableObject
    {
        public Animation animation;
        public Sprite icon;
        public string emoteName;
    }
}