// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT


using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typed.TheTapText
{
    public class TextController : MonoBehaviour
    {
        [HideInInspector] public bool isUpdated = false;

        [HideInInspector] public float AnimationProcess = 0;

        [SerializeField] VisualEffect _vfx;
        [SerializeField] Animator  _animator;

        TypedTheTapTextRealityManager _manager;

        void Start()
        {
            _manager = FindObjectOfType<TypedTheTapTextRealityManager>();
        }

        void Update()
        {
            if (isUpdated)
            {
                _vfx.SetVector3("ThumbPosition", _manager.ThumbJoint.position);
                _vfx.SetVector3("IndexPosition", _manager.IndexJoint.position);
                //_vfx.SetFloat("AnimationProcess", AnimationProcess);
            }
            else
            {
                //_vfx.SetFloat("AnimationProcess", 0);
                //_vfx.SetFloat("TensityMultipier", 0);
            }
        }

        public void OnLoaded()
        {
            _animator.SetTrigger("Loaded");
        }
    }
}
