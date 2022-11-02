using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class TheForceObjectController : MonoBehaviour
    {
        Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void BeIntaken()
        {
            _animator.SetTrigger("Intake");
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = Vector3.up * 10;

            StartCoroutine(WaitAndSetInvisible());
        }

        public void BeReleased()
        {
            _animator.SetTrigger("Release");
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        IEnumerator WaitAndSetInvisible()
        {
            yield return new WaitForSeconds(0.5f);
            transform.GetComponent<MeshRenderer>().enabled = false;
        }

        private void Update()
        {
            var speed = GetComponent<Rigidbody>().velocity;
            if (speed.magnitude > 3)
            {
                speed = speed.normalized * 3f;
            }
        }
    }
}