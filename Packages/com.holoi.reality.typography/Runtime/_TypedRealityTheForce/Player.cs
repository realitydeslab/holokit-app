using System;
using UnityEngine;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class Player : MonoBehaviour
    {
        Transform _centerEye;
        [SerializeField] HandGestureManager _HGM;
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _magicCube;
        [SerializeField] Transform _indicator;

        public event Action OnCastSomthingAction;
        public event Action OnCastNothingAction;
        public event Action<State> OnSwitchState;

        public enum State
        {
            nothing,
            something
        }

         public State _state = State.nothing;

        private void OnEnable()
        {
            OnSwitchState += SetIndicator;
        }
        private void OnDisable()
        {
            OnSwitchState += SetIndicator;
        }
        private void Start()
        {
            _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        private void Update()
        {
            var dir = (_handJoint.position - _centerEye.position).normalized;
            transform.LookAt(transform.position + dir);
            Debug.DrawRay(_centerEye.position, dir * 10f);

            switch (_state)
            {
                case State.nothing:
                    int layerMaskAttractable = LayerMask.GetMask("TheForceLayer");
                    Ray ray = new Ray(_centerEye.position, dir);
                    RaycastHit hit;

                    if (_HGM.CurrentGesture == HandGestureManager.Gesture.Palm)
                    {

                    }
                    if (Physics.Raycast(ray, out hit, 10.0f, layerMaskAttractable))
                    {
                        //Debug.Log("cast on object now");
                        // cast on object now
                        OnCastSomthingAction?.Invoke();

                        if (_HGM.CurrentGesture == HandGestureManager.Gesture.Fist)
                        {
                            _magicCube = hit.transform;
                            _magicCube.GetComponent<MagicCube>().BeAttracted();
                            _state = State.something;
                        }
                    }
                    else
                    {
                        //Debug.Log("cast nothing");

                        OnCastNothingAction?.Invoke();
                    }
                    break;
                case State.something:
                    if (_HGM.CurrentGesture == HandGestureManager.Gesture.Palm)
                    {
                        _magicCube.GetComponent<MagicCube>().BeShoot(dir);
                        _state = State.nothing;
                    }
                    break;
            }
        }


        void SetIndicator(State state)
        {
            switch (state)
            {
                case State.nothing:
                    _indicator.gameObject.SetActive(false);

                    break;
                case State.something:
                    _indicator.gameObject.SetActive(true);

                    break;
            }
        }
    }

}
