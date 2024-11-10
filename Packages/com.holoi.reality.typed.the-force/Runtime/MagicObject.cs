// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;

namespace Holoi.Reality.Typed.TheForce
{
    public class MagicObject : MonoBehaviour
    {
        Animator _animator;

        public enum State
        {
            idle,
            intaking,
            intaked
        }

        public State state;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            FindObjectOfType<MagicCube>().MagicObject = transform;
        }

        private void Update()
        {
            var speed = GetComponent<Rigidbody>().velocity;
            if (speed.magnitude > 3)
            {
                speed = speed.normalized * 3f;
            }

            if (transform.position.y < -2f)
            {
                transform.position = HoloKit.HoloKitCameraManager.Instance.CenterEyePose.position +
                    HoloKit.HoloKitCameraManager.Instance.CenterEyePose.forward;
            }

            switch (state)
            {
                case State.idle:
                    break;
                case State.intaking:
                    break;
                case State.intaked:
                    break;
            }
        }

        public void BeIntaken()
        {
            _animator.SetTrigger("Intake");
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = Vector3.up * 10;

            StartCoroutine(InvisableAfterTime());
        }

        IEnumerator InvisableAfterTime()
        {
            yield return new WaitForSeconds(0.5f);
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void BeReleased()
        {
            _animator.SetTrigger("Release");
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}