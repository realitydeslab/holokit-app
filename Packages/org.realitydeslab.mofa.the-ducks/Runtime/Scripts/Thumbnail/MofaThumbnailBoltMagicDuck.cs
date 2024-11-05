// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using RealityDesignLab.Library.HoloKitApp;


namespace RealityDesignLab.MOFA.TheDucks
{
    public class ThumbnailDuckMagicController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _duckVfx;
        [SerializeField] private Animator _animator;

        private void OnBeingHit()
        {
            _animator.SetTrigger("Hit");
            _duckVfx.SendEvent("OnHit");
            _duckVfx.SetBool("Duck Alive", false);
        }
    }
}
