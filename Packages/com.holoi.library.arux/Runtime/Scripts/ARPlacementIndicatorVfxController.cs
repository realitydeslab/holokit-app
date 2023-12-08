// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.ARUX
{
    public class ARPlacementIndicatorVfxController : MonoBehaviour
    {
        [SerializeField] private Texture2D _trueTexture;

        [SerializeField] private Texture2D _falseTexture;

        private VisualEffect _vfx;

        private Animator _animator;

        private void Start()
        {
            _vfx = GetComponent<VisualEffect>();
            _animator = GetComponent<Animator>();
        }

        public void OnFoundPlane()
        {
            _animator.SetBool("Active", true);
            _vfx.SetTexture("MainTex", _trueTexture);
            //_vfx.SetBool("IsHit", true);
        }

        public void OnLostPlane()
        {
            _animator.SetBool("Active", false);
            _vfx.SetTexture("MainTex", _falseTexture);
            //_vfx.SetBool("IsHit", false);
        }

        public void OnDeath()
        {
            _animator.SetTrigger("Die");
        }
    }
}
