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
            Debug.Log($"[MofaAI] OnNetworkSpawned with ownership {OwnerClientId} and team {Team.Value}");

            var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            mofaRealityManager.SetPlayer(999, this);

            // Spawn avatar on each client locally
            SpawnAvatar();
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
            if (IsServer)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(HoloKitCamera.Instance.CenterEyePose.position - transform.position);
                transform.rotation = MofaUtils.GetHorizontalRotation(lookAtRotation);
            }

            // Set the avatar's transform on each client manually
            _avatar.transform.rotation = transform.rotation;
        }
    }
}
