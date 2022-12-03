using System.Collections;
using UnityEngine;

namespace Holoi.Reality.Typography
{
    public class MagicObject : MonoBehaviour
    {
        Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            FindObjectOfType<MagicCube>().MagicObject = transform;
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