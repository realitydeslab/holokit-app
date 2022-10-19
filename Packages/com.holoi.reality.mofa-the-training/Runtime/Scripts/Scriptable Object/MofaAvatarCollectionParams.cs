using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFATheTraining
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAvatarCollectionParams")]
    public class MofaAvatarCollectionParams : ScriptableObject
    {
        public MetaAvatarCollection MetaAvatarCollection;

        public float Scale;

        // From avatar center to life shield center
        public Vector3 LifeShiledOffset;

        // From avatar center to avatar's center eye
        public Vector3 CenterEyeOffset;
    }
}
