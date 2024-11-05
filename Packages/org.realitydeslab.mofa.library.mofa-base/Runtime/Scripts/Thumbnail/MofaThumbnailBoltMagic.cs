// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using RealityDesignLab.Library.HoloKitApp;

namespace RealityDesignLab.MOFA.Library.MOFABase
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
