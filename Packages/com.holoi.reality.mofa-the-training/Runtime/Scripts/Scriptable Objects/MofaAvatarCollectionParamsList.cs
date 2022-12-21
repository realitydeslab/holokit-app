using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFATheTraining
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAvatarCollectionParamsList")]
    public class MofaAvatarCollectionParamsList : ScriptableObject
    {
        public List<MofaAvatarCollectionParams> List;

        public MofaAvatarCollectionParams GetAvatarCollectionParams(MetaAvatarCollection avatarCollection)
        {
            foreach (var avatarCollectionParams in List)
            {
                if (avatarCollectionParams.MetaAvatarCollection.Equals(avatarCollection))
                {
                    return avatarCollectionParams;
                }
            }
            return null;
        }
    }
}
