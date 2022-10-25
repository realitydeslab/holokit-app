using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        public Transform HandJoint;
        public GameObject WeaponPrefab;

        void Start()
        {
            var go = Instantiate(WeaponPrefab, HandJoint);
            go.transform.localPosition = new Vector3(-.025f, 0.05f, 0.01f);
            go.transform.localEulerAngles = new Vector3(-90, 0, 0);
            go.transform.localScale = Vector3.one * 0.2f;
        }
    }
}

