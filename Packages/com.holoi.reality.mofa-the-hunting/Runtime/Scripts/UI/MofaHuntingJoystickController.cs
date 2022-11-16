using UnityEngine;
using UnityEngine.Events;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingJoystickController : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;

        private bool _isEmptyInput = true;

        public UnityEvent<Vector2> OnAxisChanged;

        private void Update()
        {
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
