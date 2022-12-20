using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheTraining
{
    /// <summary>
    /// This partial class is responsible for controlling AI player's avatar.
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
                transform.SetPositionAndRotation(position, rotation);
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
            _avatarAnimator = _avatar.GetComponent<Animator>();
            _avatarAnimator.runtimeAnimatorController = _mofaAvatarAnimatorController;
            _avatarAnimator.applyRootMotion = false;
        }
    }
}
