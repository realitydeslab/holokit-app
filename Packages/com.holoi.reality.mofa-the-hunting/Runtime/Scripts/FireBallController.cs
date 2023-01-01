using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : MonoBehaviour
    {
        [SerializeField] Animator _animator;

        private const string TagName = "Plane";

        private void Start()
        {
            StartCoroutine(WaitAndDestory(6f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagName))
            {
                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;
                _animator.SetTrigger("Explode");
                StartCoroutine(WaitAndDestory(1f));
            }
        }

        IEnumerator WaitAndDestory(float time)
        {
            yield return new WaitForSeconds(time);
            _animator.SetTrigger("Die");
            Destroy(gameObject, 1f);
        }
    }
}
