using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Holoi.AssetFoundation;
using HoloKit;
using Holoi.Mofa.Base;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaAI : MofaPlayer
    {
        public MetaAvatarCollectionList AvatarList;

        private GameObject _avatar;

        private MofaBaseRealityManager _mofaRealityManager;

        private Vector3 _initialPos;

        private Vector3 _destPos;

        private float _speed = 0.3f;

        protected override void Awake()
        {
            base.Awake();

            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Spawn avatar on each client locally
            SpawnAvatar();

            if (IsServer)
            {
                var mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                mofaRealityManager.SpawnLifeShield(OwnerClientId);

                _initialPos = transform.position;
                _destPos = transform.position;
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

        protected override void FixedUpdate()
        {
            // Update NetworkTransform
            if (IsServer)
            {
                // Rotation
                Vector3 lookAtVector = HoloKitCamera.Instance.CenterEyePose.position - transform.position;
                if (lookAtVector != Vector3.zero)
                {
                    Quaternion lookAtRotation = Quaternion.LookRotation(lookAtVector);
                    transform.rotation = MofaUtils.GetHorizontalRotation(lookAtRotation);
                }

                // Position
                if (_mofaRealityManager.Phase.Value == MofaPhase.Fighting)
                {
                    if (Vector3.Distance(transform.position, _destPos) < 0.1f)
                    {
                        // Find a new destination position
                        GetNextDestPos();
                    }
                    else
                    {
                        // Approaching to the destination
                        transform.position += _speed * Time.fixedDeltaTime * (_destPos - transform.position).normalized;
                    }
                }
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

        private void GetNextDestPos()
        {
            var horizontalForward = MofaUtils.GetHorizontalForward(transform);
            var horizontalRight = MofaUtils.GetHorizontalRight(transform);
            float forwardVar = Random.Range(-3f, 3f);
            float rightVar = Random.Range(-3f, 3f);

            _destPos = _initialPos + horizontalForward * forwardVar + horizontalRight * rightVar;
        }

        protected override void OnPhaseChanged(MofaPhase mofaPhase)
        {
            base.OnPhaseChanged(mofaPhase);

            if (IsServer)
            {
                if (mofaPhase == MofaPhase.Fighting)
                {

                }
                else if (mofaPhase == MofaPhase.RoundOver)
                {

                }
            }
        }

        private IEnumerator StartSpellAI()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
