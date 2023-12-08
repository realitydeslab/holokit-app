// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using BoidsSimulationOnGPU;

namespace Holoi.Reality.Typed.TheFlock 
{
    public class BoidThumbnailManager : MonoBehaviour
    {
        [Header("Boid Objects")]
        [SerializeField] VisualEffect _boidVFX;
        [SerializeField] GameObject _boidGPU;
        [Header("Animation Objects")]
        [SerializeField] Animator _parentAnimator;
        [SerializeField] Animator _avatarAnimator;

        int n = 0;

        void Start()
        {
            _boidVFX.enabled = false;
            //StartCoroutine(WaitAndTurnRight());
        }

        private void Update()
        {
            if (n == 0)
            {
                n++;
                _boidVFX.enabled = true;
            }

            SetVfxBuffer();
        }

        void SetVfxBuffer()
        {
            if (_boidVFX != null)
            {
                _boidVFX.SetGraphicsBuffer("PositionDataBuffer", _boidGPU.GetComponent<GPUBoids>().GetBoidPositionDataBuffer());
                _boidVFX.SetGraphicsBuffer("VelocityDataBuffer", _boidGPU.GetComponent<GPUBoids>().GetBoidVelocityDataBuffer());
            }
        }

        IEnumerator WaitAndTurnRight()
        {
            // time per circle = 360/30 = 12s;
            yield return new WaitForSeconds(3f);
            TurnRight();
            StartCoroutine(WaitAndTurnRight());
            yield return new WaitForSeconds(1.5f);
            _parentAnimator.SetTrigger("Idle");

        }

        void TurnRight()
        {
            Debug.Log("turn right");
            _parentAnimator.SetTrigger("Turn Right");
            _avatarAnimator.SetTrigger("Turn Right");
        }
    }
}