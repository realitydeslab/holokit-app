// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(Animator))]
    public class MofaThumbnailAvatar : MonoBehaviour
    {
        [SerializeField] private Transform _parent;

        [SerializeField] private Vector2 _avatarVelocity;

        [SerializeField] private GameObject _boltPrefab;

        [SerializeField] private Vector3 _boltMagicVelocity = new(0,0,3);

        [SerializeField] private float _attackDelay = 0f;

        [SerializeField] private float _attackChargeTime = 3f;

        private Animator _avatarAnimator;

        private float _attackCurrentCharge;

        private readonly Queue<GameObject> _pool = new();

        private void Start()
        {
            _avatarAnimator = GetComponent<Animator>();

            _avatarAnimator.SetFloat("VelocityZ", _avatarVelocity.x);
            _avatarAnimator.SetFloat("VelocityX", _avatarVelocity.y);

            _attackCurrentCharge = _attackChargeTime  - _attackDelay;
        }

        private void Update()
        {
            _attackCurrentCharge += Time.deltaTime;
            if (_attackCurrentCharge >= _attackChargeTime)
            {
                _avatarAnimator.SetTrigger("Attack");
                _attackCurrentCharge = 0f;
            }
        }

        private void AddObjectToQueue(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var bolt = Instantiate(_boltPrefab, _parent);
                bolt.GetComponent<MofaThumbnailBoltMagic>().SetPool(this);
                bolt.SetActive(false);
                _pool.Enqueue(bolt);
            }
        }

        public void ReturnObjectToQueue(GameObject go)
        {
            go.SetActive(false);
            _pool.Enqueue(go);
        }

        private void ShootBolt()
        {
            if (_pool.Count == 0)
            {
                AddObjectToQueue(1);
            }
            var bolt = _pool.Dequeue();
            bolt.SetActive(true);
            // Rotation
            bolt.transform.LookAt(transform.forward * 5f);
            // Position
            bolt.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1f;
            // Add velocity
            bolt.GetComponent<Rigidbody>().linearVelocity = (transform.forward * _boltMagicVelocity.z)  + (transform.up * _boltMagicVelocity.y)
                + (transform.right * _boltMagicVelocity.x);
        }

        #region Animation Event Receivers
        public void FootL() { }

        public void FootR() { }

        public void Hit() => ShootBolt();
        #endregion
    }
}
