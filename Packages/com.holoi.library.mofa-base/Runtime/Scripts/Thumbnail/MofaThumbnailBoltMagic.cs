// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
            GetComponent<Rigidbody>().velocity = Vector3.zero;
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
