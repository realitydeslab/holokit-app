using System;
using UnityEngine;

namespace Holoi.Reality.MOFATheGhost.UI
{
    public class MofaGhostJoystickController : MonoBehaviour
    {
        private Joystick _joystick;

        private bool _isEmptyInput = true;

        public static event Action<Vector2> OnAxisChanged;

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
                    //OnAxisChanged?.Invoke(_joystick.Direction);
                }
            }
            else // If the input is not empty
            {
                if (_isEmptyInput)
                {
                    _isEmptyInput = false;
                }
                //Debug.Log($"[Joystick] {_joystick.Direction}");
                OnAxisChanged?.Invoke(_joystick.Direction);
            }
        }
    }
}
