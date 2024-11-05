// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.Avatar.Meebits
{
    public class MeebitsController : MonoBehaviour
    {
        [Header("Emission")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _isEmisive;
        private SkinnedMeshRenderer _meshRenderer;
        public float EmissionIntensity = 1f;

        [Header("Weapon")]
        [Tooltip("if false, do not need to modifiy following variables.")]
        [SerializeField] bool _holdWeapon;
        public Transform HandJoint;
        public GameObject WeaponPrefab;
        public Vector3 Offset= new Vector3(-.025f, 0.05f, 0.01f);
        public float Scale = 0.2f;

        [Header("Clip")]
        [SerializeField] private bool _playFadeInAnimation = true;

        private int _clipProcessID;

        private void Start()
        {
            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            // Add weapon to Meebits
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

            // Set brightness
            if (_isEmisive)
            {
                var mats = _meshRenderer.materials;
                foreach (var mat in mats)
                {
                    //mat.SetColor("_EmissionColor", new Color(Emission, Emission, Emission, 1));
                    mat.SetFloat("_Brightness", EmissionIntensity);
                }
            }

            // Play fade in animation (clip)
            _clipProcessID = Shader.PropertyToID("_ClipProcess");
            if (_playFadeInAnimation)
            {
                FadeInAnimation();
            }
            else
            {
                foreach (var mat in _meshRenderer.materials)
                {
                    mat.SetFloat(_clipProcessID, 1f);
                    mat.SetVector("_ClipRange", new Vector2(-0.6f, -0.6f + (1.8f * transform.localScale.x)));
                }
            }
        }

        private void FadeInAnimation()
        {
            // Set initial clip value
            foreach (var mat in _meshRenderer.materials)
            {
                mat.SetFloat(_clipProcessID, 0f);
                mat.SetVector("_ClipRange", new Vector2(-0.6f, -0.6f + (1.8f * transform.localScale.x)));
            }

            LeanTween.value(0f, 1f, 1f)
                .setOnUpdate((float t) =>
                {
                    foreach (var mat in _meshRenderer.materials)
                    {
                        mat.SetFloat(_clipProcessID, t);
                    }
                });
        }
    }
}
