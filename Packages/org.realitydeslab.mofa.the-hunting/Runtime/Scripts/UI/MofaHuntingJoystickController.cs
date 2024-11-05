// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.Events;

namespace RealityDesignLab.MOFA.TheHunting.UI
{
    public class MofaHuntingJoystickController : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;

        private bool _isEmptyInput = true;

        public UnityEvent<Vector2> OnAxisChanged;

        private void Update()
        {
            // If the joystick input is empty
            if (_joystick.Horizontal == 0f && _joystick.Vertical == 0f)
            {
                if (!_isEmptyInput)
                {
                    _isEmptyInput = true;
                    OnAxisChanged?.Invoke(_joystick.Direction);
                }
            }
            else
            {
                if (_isEmptyInput)
                {
                    _isEmptyInput = false;
                }
                OnAxisChanged?.Invoke(_joystick.Direction);
            }
        }
    }
}
