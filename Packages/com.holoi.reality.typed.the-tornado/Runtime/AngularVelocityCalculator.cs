// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typed.TheTornado
{
    public class AngularVelocityCalculator : MonoBehaviour
    {
        [SerializeField] Transform _target;

        float _angularVelocityY;

        [HideInInspector] public Vector3 Rotation;

        public float AngularVelocityY
        {
            get { return _angularVelocityY; }
            set { _angularVelocityY = value; }
        }

        void FixedUpdate()
        {
            RotationUpdate();
        }

        void RotationUpdate()
        {
            var targetDirection = _target.forward;
            var direction = transform.forward;

            var signedAngleY = Vector3.SignedAngle(direction, targetDirection, Vector3.up);

            var m = Mathf.Abs(signedAngleY) / 90f;

            var angularSteer = (signedAngleY - _angularVelocityY) * m;

            var angularACC = angularSteer;

            Debug.Log($"signedAngleY: { signedAngleY }");
            Debug.Log($"angularACC: { angularSteer }");

            if (signedAngleY > 0)
            {
                _angularVelocityY += angularACC;
                if (_angularVelocityY > 90) _angularVelocityY = 90;
                Rotation += Vector3.up * _angularVelocityY * Time.deltaTime;
                Debug.Log($"_angularVelocityY: { _angularVelocityY }");
            }
            else
            {
                _angularVelocityY += angularACC;
                if (_angularVelocityY < -90) _angularVelocityY = -90;
                Rotation += Vector3.up * _angularVelocityY * Time.deltaTime;
                Debug.Log($"_angularVelocityY: { _angularVelocityY }");
            }

            transform.rotation = Quaternion.Euler(Rotation);
        }
    }
}
