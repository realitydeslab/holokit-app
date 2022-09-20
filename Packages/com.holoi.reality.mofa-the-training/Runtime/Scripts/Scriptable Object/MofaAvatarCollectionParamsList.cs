using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MOFATheTraining
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MofaAvatarCollectionParamsList")]
    public class MofaAvatarCollectionParamsList : ScriptableObject
    {
        public List<MofaAvatarCollectionParams> List;
    }
}
