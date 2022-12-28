using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : MonoBehaviour
    {
        [SerializeField] Animator _animator;

        private const string TagName = "Plane";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagName))
            {
                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;
                _animator.SetTrigger("Explode");
                StartCoroutine(WaitAndDestory());
                
            }
        }

        IEnumerator WaitAndDestory()
        {
            yield return new WaitForSeconds(1f);
            _animator.SetTrigger("Die");
            Destroy(gameObject, 1f);
        }
    }
}
