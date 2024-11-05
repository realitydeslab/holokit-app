// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using BoidsSimulationOnGPU;

namespace RealityDesignLab.Typed.TheFlock 
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