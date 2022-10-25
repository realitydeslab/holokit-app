using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        public Transform HandJoint;
        public Transform WeaponPrefab;

        void Start()
        {
            var go = Instantiate(WeaponPrefab, HandJoint);
            go.localPosition = new Vector3(0, 0.05f, 0.05f);
            go.localEulerAngles = new Vector3(-90, 0, 0);
        }
    }
}

