// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.TheTraining
{
    /// <summary>
    /// This partial class is responsible for avatar spawning.
    /// </summary>
    public partial class MofaAIPlayer
    {
        /// <summary>
        /// Different NFT avatar collection has different param set.
        /// </summary>
        [SerializeField] private MofaAvatarCollectionParamsList _mofaAvatarCollectionParamsList;

        /// <summary>
        /// For avatar animation control.
        /// </summary>
        [SerializeField] private RuntimeAnimatorController _mofaAvatarAnimatorController;

        // SH_Meebits fails Metal pipeline creation on the upgraded iOS player.
        // Keep the AI visible while retaining each avatar material's base colour.
        [SerializeField] private Material _iosAvatarFallbackMaterial;

        private readonly List<Material> _avatarMaterialOverrides = new();
        public int AvatarFallbackMaterialCount => _avatarMaterialOverrides.Count;

        public Animator AvatarAnimator => _avatarAnimator;

        /// <summary>
        /// The reference to the AI player's avatar
        /// </summary>
        private GameObject _avatar;

        /// <summary>
        /// The reference of the avatar's animator
        /// </summary>
        private Animator _avatarAnimator;

        /// <summary>
        /// The offset from the avatar's origin to the avatar's center eye.
        /// </summary>
        private Vector3 _avatarOriginToCenterEyeOffset;

        /// <summary>
        /// Spawn a specific avatar on each client device.
        /// </summary>
        /// <param name="avatarCollectionBundleId">The collection bundle Id of the avatar</param>
        /// <param name="avatarTokenId">The token Id of the avatar</param>
        /// <param name="position">The initial position of the avatar</param>
        /// <param name="rotation">The initial rotation of the avatar</param>
        [ClientRpc]
        public void SpawnAvatarClientRpc(string avatarCollectionBundleId, string avatarTokenId, Vector3 position, Quaternion rotation)
        {
            if (IsServer)
            {
                InitialPosition = position;
                InitialForward = rotation * Vector3.forward;
                transform.SetPositionAndRotation(position, rotation);
            }
            SpawnAvatar(avatarCollectionBundleId, avatarTokenId);
        }

        private void SpawnAvatar(string avatarCollectionBundleId, string avatarTokenId)
        {
            if (_avatar != null)
            {
                Debug.LogError("[MofaAIPlayer] Cannot spawn AI avatar multiple times");
                return;
            }

            // Grab avatar metadata
            var preferencedAvatarCollection = HoloKitApp.Instance.GlobalSettings.AvatarCollectionList.GetMetaAvatarCollection(avatarCollectionBundleId);
            var preferencedAvatar = preferencedAvatarCollection.GetMetaAvatar(avatarTokenId);
            var avatarCollectionParams = _mofaAvatarCollectionParamsList.GetAvatarCollectionParams(preferencedAvatarCollection);
            _avatarOriginToCenterEyeOffset = avatarCollectionParams.AvatarOriginToCenterEyeOffset;
            CenterEyeToLifeShieldOffset = avatarCollectionParams.CenterEyeToLifeShiledOffset;

            // Spawn the avatar
            _avatar = Instantiate(preferencedAvatar.Prefab, transform.position, transform.rotation);
            _avatar.transform.SetParent(transform);
            _avatar.transform.localPosition = Vector3.zero;
            _avatar.transform.localRotation = Quaternion.identity;
            _avatar.transform.localScale = new(avatarCollectionParams.Scale, avatarCollectionParams.Scale, avatarCollectionParams.Scale);
            ApplyIOSAvatarMaterialFallback();
            _avatarAnimator = _avatar.GetComponent<Animator>();
            _avatarAnimator.runtimeAnimatorController = _mofaAvatarAnimatorController;
            _avatarAnimator.applyRootMotion = false;
            _avatarAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            _avatar.AddComponent<MofaAvatarAnimationEventHandler>();
        }

        private void ApplyIOSAvatarMaterialFallback()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;
            if (_iosAvatarFallbackMaterial == null)
            {
                Debug.LogError("[MofaAIPlayer] iOS avatar fallback material is missing.");
                return;
            }
            foreach (var renderer in _avatar.GetComponentsInChildren<Renderer>(true))
            {
                var materials = renderer.sharedMaterials;
                var changed = false;
                for (var i = 0; i < materials.Length; i++)
                {
                    var original = materials[i];
                    if (original == null || original.shader == null ||
                        original.shader.name != "Shader Graphs/SH_Meebits") continue;
                    var replacement = new Material(_iosAvatarFallbackMaterial)
                    {
                        name = original.name + " (iOS fallback)"
                    };
                    if (original.HasProperty("_BaseColor"))
                        replacement.SetColor("_BaseColor", original.GetColor("_BaseColor"));
                    if (original.HasProperty("_BaseMap") && original.GetTexture("_BaseMap") != null)
                        replacement.SetTexture("_BaseMap", original.GetTexture("_BaseMap"));
                    replacement.SetFloat("_Smoothness", 0.1f);
                    _avatarMaterialOverrides.Add(replacement);
                    materials[i] = replacement;
                    changed = true;
                }
                if (changed) renderer.sharedMaterials = materials;
            }
            Debug.Log($"[MofaAIPlayer] iOS avatar fallback applied to {_avatarMaterialOverrides.Count} materials.");
        }

        private void ReleaseAvatarMaterialOverrides()
        {
            foreach (var material in _avatarMaterialOverrides)
                if (material != null) Destroy(material);
            _avatarMaterialOverrides.Clear();
        }
    }
}
