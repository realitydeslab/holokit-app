// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;

namespace Holoi.Reality.MOFA.TheGhost.UI
{
    public class MofaGhostJoystickController : MonoBehaviour
    {
        private Joystick _joystick;

        private bool _isEmptyInput = true;

        public static event Action<Vector2> OnAxisChanged;

        public static event Action OnInputStarted;

        public static event Action OnInputStopped;

        private void Start()
        {
            _joystick = GetComponent<Joystick>();
        }

        private void Update()
        {
            // If the joystick input is empty
            if (_joystick.Horizontal == 0f && _joystick.Vertical == 0f)
            {
                if (!_isEmptyInput)
                {
                    _isEmptyInput = true;
                    OnInputStopped?.Invoke();
                    //OnAxisChanged?.Invoke(_joystick.Direction);
                }
            }
            else // If the input is not empty
            {
                if (_isEmptyInput)
                {
                    _isEmptyInput = false;
                    OnInputStarted?.Invoke();
                }
                //Debug.Log($"[Joystick] {_joystick.Direction}");
                OnAxisChanged?.Invoke(_joystick.Direction);
            }
        }
    }
}