using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Reality.MOFATheTraining
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private AssetFoundation.Reality _reality;

        [SerializeField] private MetaAvatarCollection _avatarCollection;

        [SerializeField] private MetaObjectCollection _objectCollection;

        private void Start()
        {
            Debug.Log($"Avatar collection compatibility {_reality.IsCompatibleWithMetaAvatarCollection(_avatarCollection)}");
            //Debug.Log($"Object collection compatibility {_reality.IsCompatibleWithMetaObjectCollection(_objectCollection)}");
        }
    }
}
