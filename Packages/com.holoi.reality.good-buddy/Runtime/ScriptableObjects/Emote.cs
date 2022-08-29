using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Realities.GoodBuddy
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Emote")]
    public class Emote: ScriptableObject
    {
        public Animation animation;
        public Sprite icon;
        public string name;
    }
}