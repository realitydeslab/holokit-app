using System;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Reality.MOFATheHunting
{
    public class LockTargetButton : MonoBehaviour
    {
        [SerializeField] private Image _image;

        private bool _isLocked;

        public static event Action<bool> OnLockTargetButtonPressed;

        private void Start()
        {
            _isLocked = false;
            _image.color = Color.gray;
        }

        public void OnButtonPressed()
        {
            if (_isLocked)
            {
                _isLocked = false;
                _image.color = Color.gray;
            }
            else
            {
                _isLocked = true;
                _image.color = Color.white;
            }
            OnLockTargetButtonPressed?.Invoke(_isLocked);
        }
    }
}
