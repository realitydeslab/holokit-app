using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Button List")]
        [SerializeField] MyButton _forwardButton;
        [SerializeField] MyButton _backwardButton;
        [SerializeField] MyButton _rightwardButton;
        [SerializeField] MyButton _leftwardButton;

        float _vZ;
        float _vX;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_forwardButton.buttonPressed)
            {
                Debug.Log("_forwardButton");
                _vZ = 1.51f * Time.deltaTime;
            }
            else if (_backwardButton.buttonPressed)
            {
                Debug.Log("_backwardButton");

                _vZ = -1.51f * Time.deltaTime;

            }
            else if (_rightwardButton.buttonPressed)
            {
                Debug.Log("_rightwardButton");

                _vX = 1.51f * Time.deltaTime;
            }
            else if (_leftwardButton.buttonPressed)
            {
                Debug.Log("_leftwardButton");

                _vX = -1.51f * Time.deltaTime;
            }
            else
            {

            }


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
                Vector3 distFromLastFrame = transform.position - _lastFramePosition;
                if (distFromLastFrame != Vector3.zero)
                {
                    var staticThreshold = 0.001667f; // if velocity < 0.1m/s, we regard it static.
                    _vZ = InverseClamp(_vZ, -1 * staticThreshold, 1 * staticThreshold);
                    _vX = InverseClamp(_vX, -1 * staticThreshold, 1 * staticThreshold);

                    _vZ = Remap(_vZ, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);
                    _vX = Remap(_vX, _velocityRemap.x, _velocityRemap.y, _velocityRemap.z, _velocityRemap.w, true);

                    _animator.SetFloat("Velocity Z", _vZ);
                    _animator.SetFloat("Velocity X", _vX);
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
