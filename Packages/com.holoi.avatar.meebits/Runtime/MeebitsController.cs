using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        public Transform HandJoint;
        public GameObject WeaponPrefab;
        public Vector3 Offset= new Vector3(-.025f, 0.05f, 0.01f);
        public float Scale = 0.2f;

        void Start()
        {
            var go = Instantiate(WeaponPrefab, HandJoint);
            go.transform.localPosition = Offset;
            go.transform.localEulerAngles = new Vector3(-90, 0, 0);
            go.transform.localScale = Vector3.one * Scale;
        }
    }
}

