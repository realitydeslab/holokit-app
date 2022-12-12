using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        [Header("Emission")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _emission;
        public SkinnedMeshRenderer MeshRenderer;
        public float Emission = 0.1f;

        [Header("Weapon")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _holdWeapon;
        public Transform HandJoint;
        public GameObject WeaponPrefab;
        public Vector3 Offset= new Vector3(-.025f, 0.05f, 0.01f);
        public float Scale = 0.2f;




        void Start()
        {
            if (_holdWeapon)
            {
                if (HandJoint != null && WeaponPrefab != null)
                {
                    var weaponInstance = Instantiate(WeaponPrefab, HandJoint);
                    weaponInstance.transform.localPosition = Offset;
                    weaponInstance.transform.localEulerAngles = new Vector3(-90, 0, 0);
                    weaponInstance.transform.localScale = Vector3.one * Scale;
                }
            }


            if (_emission)
            {
                if (MeshRenderer)
                {
                    var mats = MeshRenderer.materials;
                    foreach (var mat in mats)
                    {
                        mat.SetColor("_EmissionColor", new Color(Emission, Emission, Emission, 1));
                    }
                }
            }
        }
    }

}

