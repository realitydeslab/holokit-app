// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.MOFA.TheDucks
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
