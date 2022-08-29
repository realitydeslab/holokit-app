using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/TechTags")]
    public class TechTag : ScriptableObject
    {
        public string name;
    }
}