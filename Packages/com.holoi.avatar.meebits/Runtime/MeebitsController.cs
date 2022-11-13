using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        [Header("Emission")]
        [SerializeField] bool _emission;
        public SkinnedMeshRenderer MeshRenderer;
        public float Emission = 0.1f;

        [Header("Weapon")]
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
                else
                {
                    Debug.Log("can not hold weapon when Handjoint||WeaponPrefab is null.");
                }
            }


            if (_emission)
            {
                if (MeshRenderer)
                {
                    var mats = MeshRenderer.materials;
                    foreach (var mat in mats)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", new Color(Emission, Emission, Emission, 1));
                    }
                }
                else
                {
                    Debug.Log("Can not adjust emission when meshrenderer is null.");
                }

            }
        }
    }

}

