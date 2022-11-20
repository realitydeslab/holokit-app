using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class SpellUniformLinearMotionController : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void FixedUpdate()
        {
            transform.position += _speed * Time.fixedDeltaTime * transform.forward;
        }
    }
}