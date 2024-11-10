// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typed.TheTapText
{
    public class TypedTheTapTextRealityManager : RealityManager
    {
        public Transform ThumbJoint;

        public Transform IndexJoint;

        [SerializeField] GameObject _textPrefab;

        GameObject _textInstance;

        float _creationProcess = 0;

        enum State
        {
            Idle,
            Creating,
            Coolingdown
        }

        State _state = State.Idle;

        void Start()
        {
            HoloKit.HoloKitHandTracker.Instance.IsActive = true;
        }

        void Update()
        {
            switch (_state)
            {
                case State.Idle:
                    //Debug.Log("idle");
                    if (HoloKit.HoloKitHandTracker.Instance.IsActive)
                    {
                        var distance = Vector3.Distance(ThumbJoint.position, IndexJoint.position);
                        if (distance > 0.02f)
                        {
                            _textInstance = Instantiate(_textPrefab);
                            _textInstance.GetComponent<TextController>().isUpdated = true;
                            _state = State.Creating;
                        }
                    }
                    break;
                case State.Creating:
                    //Debug.Log("Creating");

                    if (HoloKit.HoloKitHandTracker.Instance.IsActive)
                    {
                        var distance = Vector3.Distance(ThumbJoint.position, IndexJoint.position);
                        if (distance > 0.1f && _textInstance.GetComponent<TextController>().isUpdated)
                        {
                            _creationProcess += Time.deltaTime * 0.5f;
                            if (_creationProcess > 0.5f)
                            {
                                _creationProcess = 0.5f;
                                _textInstance.GetComponent<TextController>().isUpdated = false;
                                _textInstance.GetComponent<TextController>().OnLoaded();
                                StartCoroutine(WaitAndSwitchToIdle());
                                _state = State.Coolingdown;
                            }
                        }
                        else
                        {
                            _creationProcess -= Time.deltaTime * 1f;
                            if (_creationProcess < 0) _creationProcess = 0;
                        }

                        _textInstance.GetComponent<TextController>().AnimationProcess = _creationProcess;
                    }
                    break;
                case State.Coolingdown:

                    _creationProcess -= Time.deltaTime * 2f;
                    if (_creationProcess < 0) _creationProcess = 0;
                    _textInstance.GetComponent<TextController>().AnimationProcess = _creationProcess;

                    break;

            }
        }

        IEnumerator WaitAndSwitchToIdle()
        {
            yield return new WaitForSeconds(1f);
            _state = State.Idle;
        }
    }
}