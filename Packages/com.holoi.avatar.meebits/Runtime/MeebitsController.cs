using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        [Header("Emission")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _isEmisive;
        public SkinnedMeshRenderer MeshRenderer;
        public float EmissionIntensity = 1f;

        [Header("Weapon")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _holdWeapon;
        public Transform HandJoint;
        public GameObject WeaponPrefab;
        public Vector3 Offset= new Vector3(-.025f, 0.05f, 0.01f);
        public float Scale = 0.2f;

        [Header("Clip")]
        //[SerializeField] bool _isClip = false;
        [Range(0,1)]
        [SerializeField] float _clipProcess = 1;

        void Start()
        {
            if (true)
            {
                if (MeshRenderer)
                {
                    var mats = MeshRenderer.materials;
                    foreach (var mat in mats)
                    {
                        mat.SetFloat("_ClipProcess", _clipProcess);
                        mat.SetVector("_ClipRange", new Vector2(-0.6f, -0.6f + (1.8f* transform.localScale.x)));
                    }
                }
            }

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


            if (_isEmisive)
            {
                if (MeshRenderer)
                {
                    var mats = MeshRenderer.materials;
                    foreach (var mat in mats)
                    {
                        //mat.SetColor("_EmissionColor", new Color(Emission, Emission, Emission, 1));
                        mat.SetFloat("_Brightness", EmissionIntensity);
                    }
                }
            }
        }
    }

}

