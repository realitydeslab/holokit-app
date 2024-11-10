// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    public class MofaThumbnailBoltMagic : MonoBehaviour
    {
        private MofaThumbnailAvatar _pool;

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetPool(MofaThumbnailAvatar pool)
        {
            _pool = pool;
        }

        private void OnTriggerEnter(Collider other)
        {
            _animator.SetTrigger("Hit");
            //GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            StartCoroutine(HoloKitAppUtils.WaitAndDo(1f, () =>
            {
                // Reset animator
                _animator.Rebind();
                _animator.Update(0f);
                _pool.ReturnObjectToQueue(gameObject);
            }));
        }
    }
}
