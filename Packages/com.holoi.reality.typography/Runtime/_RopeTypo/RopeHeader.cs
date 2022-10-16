using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class RopeHeader : MonoBehaviour
    {
        Vector3 _lastPosition;
        Vector3 _currentPosition;

        Vector3 _velocity;
        Vector3 _direction;

        void Start()
        {
            _lastPosition = Vector3.zero;


        }

        // Update is called once per frame
        void Update()
        {
            _velocity = (transform.position - _lastPosition) / Time.deltaTime;

            _direction = _velocity.normalized;

            _lastPosition = transform.position;

            FindObjectOfType<RopeControllerRealisticNoSpring>().ropeHeaderDirection = -1f * _direction;

            transform.LookAt(transform.position + _direction);
        }
    }
}
