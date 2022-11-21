using UnityEngine;

namespace Holoi.Library.MOFABase
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