using UnityEngine;
using UnityEngine.Events;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingJoystickController : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;

        public UnityEvent<Vector2> OnAxisChanged;

        private void Update()
        {
            if (_joystick.Horizontal !=0 || _joystick.Vertical != 0)
            {
                OnAxisChanged?.Invoke(new Vector2(-_joystick.Vertical, _joystick.Horizontal));
            }
        }
    }
}
