using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class AudioVFXManager : MonoBehaviour
    {
        public Transform _handPositionSample;
        [SerializeField] private float multipier;
        [SerializeField] AudioSource _as;

        bool _interactiveScale = false;
        bool _lockScale = false;

        private void OnEnable()
        {
            HoloKitHandTracker.OnHandValidityChanged += HandValidityChanged;
        }

        private void OnDisable()
        {
            HoloKitHandTracker.OnHandValidityChanged -= HandValidityChanged;
        }

        public void HandValidityChanged(bool valid)
        {
            if (valid)
            {
                _interactiveScale = true;

            }
            else
            {

            }
        }

        public void LockScale()
        {
            if (_lockScale)
            {
                _lockScale = false;

            }
            else
            {
                _lockScale = true;
            }
        }

        private void Update()
        {
            if (!_lockScale)
            {
                if (_interactiveScale)
                {
                    Vector3 handOnPlane = Vector3.ProjectOnPlane(_handPositionSample.position, HoloKitCamera.Instance.CenterEyePose.forward);

                    var dist = Vector2.Distance(Vector2.zero, new Vector2(handOnPlane.x, handOnPlane.y));

                    transform.localScale = Vector3.one * dist * multipier;
                }
                else
                {

                }

            }
            else
            {

            }


        }

        public void PlayMusic()
        {
            if (_as.isPlaying)
            {
                _as.Pause();
            }
            else
            {
                _as.Play();
            }
        }

        public void ResetMusic()
        {
            _as.Stop();
            //_as.Play();
        }
    }
}