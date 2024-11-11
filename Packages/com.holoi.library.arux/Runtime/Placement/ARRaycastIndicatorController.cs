// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.ARUX
{
    public class ARRaycastIndicatorController : MonoBehaviour
    {
        [SerializeField] private Texture2D _trueTexture;

        [SerializeField] private Texture2D _falseTexture;

        private VisualEffect _vfx;

        private Animator _animator;

        private void Start()
        {
            _vfx = GetComponent<VisualEffect>();
            _animator = GetComponent<Animator>();
            _animator.SetTrigger("Init");
        }

        public void OnFoundPlane()
        {
            _vfx.SetBool("IsHit", true);
            _vfx.SetTexture("MainTex", _trueTexture);
        }

        public void OnLostPlane()
        {
            _vfx.SetBool("IsHit", false);
            _vfx.SetTexture("MainTex", _falseTexture);
        }

        public void OnDeath()
        {
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                Destroy(gameObject);
            }));
        }
    }
}
