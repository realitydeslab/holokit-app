// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.Reality.QuantumRealm
{
    public class LSCVFXController : MonoBehaviour
    {
        Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        public void PlayDie()
        {
            _animator.SetTrigger("Fade Out");
            Disable(1);
        }

        void Disable(float time)
        {
            StartCoroutine(DisableAfterTimes(time));
        }

        IEnumerator DisableAfterTimes(float time)
        {
            yield return new WaitForSeconds(time);
            this.gameObject.SetActive(false);
        }
    }
}
