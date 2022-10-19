using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class PupprtryController : MonoBehaviour
    {
        Animator _animator;
        bool _notFirstAnimationFrame = false;
        Vector3 _lastFramePosition;
        Vector3 _lastFrameForward;
        Vector3 _lastFrameRight;
        private readonly Vector4 _velocityRemap = new(-.02f, .02f, -1f, 1f);

        public float vZ;
        public float vX;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateAvatarMovementAnimation();
        }


        private void UpdateAvatarMovementAnimation()
        {
            if (_animator == null)
            {
                return;
            }

            if (_notFirstAnimationFrame)
            {
                // Calculate the relative z and x velocity
                //Vector3 distFromLastFrame = transform.position - _lastFramePosition;
                if (true)
                {
                    var staticThreshold = 0.001667f; // if velocity < 0.1m/s, we regard it static.
                    vZ = InverseClamp(vZ, -1 * staticThreshold, 1 * staticThreshold);
                    vX = InverseClamp(vX, -1 * staticThreshold, 1 * staticThreshold);

                    vZ = Remap(vZ, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);
                    vX = Remap(vX, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);

                    _animator.SetFloat("Velocity Z", vZ);
                    _animator.SetFloat("Velocity X", vX);
                }
            }
            else
            {
                _notFirstAnimationFrame = true;
            }

            // Save data for next frame calculation
            _lastFrameForward = transform.forward;
            _lastFrameRight = transform.right;
            _lastFramePosition = transform.position;
        }

        private void PlayAvatarCastSpellAnimation()
        {
            //if (spellType == SpellType.Basic)
            //{
            _animator.SetTrigger("Attack A");
            //}
            //else
            //{
            //    _animator.SetTrigger("Attack B");
            //}
        }

        public static float InverseClamp(float value, float min, float max)
        {
            if (value < max && value > min)
            {
                Debug.Log("clamp to 0!");
                return 0f;
            }
            else
            {
                return value;
            }
        }

        public static float Remap(float input, float in_min, float in_max, float out_min, float out_max, bool isClamp)
        {
            if (isClamp)
            {
                if (input > in_max) input = in_max;
                if (input < in_min) input = in_min;
            }
            else
            {

            }
            float o = ((input - in_min) / (in_max - in_min)) * (out_max - out_min) + out_min;
            return o;
        }
    }
}
