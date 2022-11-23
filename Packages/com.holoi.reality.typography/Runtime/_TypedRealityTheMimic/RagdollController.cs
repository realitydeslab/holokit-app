using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] Transform _joint;
        public bool _force;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (_force)
            {
                _force = false;
                OnAppleForce();
            }
        }

        void OnAppleForce()
        {
            _joint.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
        }
    }
}
