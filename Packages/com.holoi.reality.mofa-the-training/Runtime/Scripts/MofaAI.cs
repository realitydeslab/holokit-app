using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.AssetFoundation;
using HoloKit;
using Holoi.Mofa.Base;
using Holoi.HoloKit.App;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAI : MofaPlayer
    {
        public MetaAvatarCollectionList AvatarList;

        private GameObject _avatar;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Spawn avatar on each client locally
            SpawnAvatar();

            if (IsServer)
            {
                var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                mofaRealityManager.SpawnLifeShield(OwnerClientId);
            }
        }

        private void SpawnAvatar()
        {
            if (_avatar == null)
            {
                _avatar = Instantiate(AvatarList.list[0].metaAvatars[0].prefab,
                transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("[AvatarController] Avatar already spawned");
            }
        }

        protected override void Update()
        {
            // Update NetworkTransform
            if (IsServer)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(HoloKitCamera.Instance.CenterEyePose.position - transform.position);
                transform.rotation = MofaUtils.GetHorizontalRotation(lookAtRotation);
            }

            // Set the avatar's transform on each client manually
            if (_avatar != null)
            {
                _avatar.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            // Set the life shield's transform on each client manually
            if (LifeShield != null)
            {
                LifeShield.transform.SetPositionAndRotation(transform.position + transform.rotation * new Vector3(0f, 1f, 0.8f),
                    transform.rotation);
            }
        }
    }
}
