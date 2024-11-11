// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.Base
{
    public class SpellUniformLinearMotionController : MonoBehaviour
    {
        [SerializeField] private float _speed;

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                _isMoving = value;
            }
        }

        private bool _isMoving;

        private void OnEnable()
        {
            _isMoving = true;
        }

        private void FixedUpdate()
        {
            if (_isMoving)
            {
                transform.position += _speed * Time.fixedDeltaTime * transform.forward;
            }
        }
    }
}