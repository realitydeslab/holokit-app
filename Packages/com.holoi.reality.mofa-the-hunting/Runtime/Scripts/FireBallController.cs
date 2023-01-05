using UnityEngine;
using System.Collections;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private FireBallController _fireBallPrefab;

        public static bool IsNextFireBallSecondaryAttack = false;

        private bool _isSecondaryAttack;

        /// <summary>
        /// Whether the rigidbody has been added force.
        /// </summary>
        private bool _beingAddedForce;

        /// <summary>
        /// Whether the fire ball is the orignal one, which is the one spawned first.
        /// </summary>
        private bool _isTheOriginalFireBall = true;

        private Rigidbody _rigidbody;

        private const string TagName = "Plane";

        private const float DeviationAngleInDeg1 = 15f;

        private const float DeviationAngleInDeg2 = 30f;

        private void Start()
        {
            _isSecondaryAttack = IsNextFireBallSecondaryAttack;
            _rigidbody = GetComponent<Rigidbody>();

            StartCoroutine(WaitAndDestory(6f));
        }

        private void Update()
        {
            if (!_isSecondaryAttack)
                return;

            if (!_isTheOriginalFireBall)
                return;

            if (!_beingAddedForce && _rigidbody.velocity != Vector3.zero)
            {
                _beingAddedForce = true;

                Vector3 forward = _rigidbody.velocity.normalized;
                float magnitude = _rigidbody.velocity.magnitude;

                // Spawn the left side fire ball 1
                var leftFireBallInstance1 = Instantiate(_fireBallPrefab);
                leftFireBallInstance1._isTheOriginalFireBall = false;
                Vector3 leftForward1 = Quaternion.Euler(0f, -DeviationAngleInDeg1, 0f) * forward;
                leftFireBallInstance1.GetComponent<Rigidbody>().velocity = magnitude * leftForward1;

                // Spawn the left side fire ball 2
                var leftFireBallInstance2 = Instantiate(_fireBallPrefab);
                leftFireBallInstance2._isTheOriginalFireBall = false;
                Vector3 leftForward2 = Quaternion.Euler(0f, -DeviationAngleInDeg2, 0f) * forward;
                leftFireBallInstance2.GetComponent<Rigidbody>().velocity = magnitude * leftForward2;

                // Spawn the right side fire ball 1
                var rightFireBallInstance1 = Instantiate(_fireBallPrefab);
                rightFireBallInstance1._isTheOriginalFireBall = false;
                Vector3 rightForward1 = Quaternion.Euler(0f, DeviationAngleInDeg1, 0f) * forward;
                rightFireBallInstance1.GetComponent<Rigidbody>().velocity = magnitude * rightForward1;

                // Spawn the right side fire ball 2
                var rightFireBallInstance2 = Instantiate(_fireBallPrefab);
                rightFireBallInstance2._isTheOriginalFireBall = false;
                Vector3 rightForward2 = Quaternion.Euler(0f, DeviationAngleInDeg2, 0f) * forward;
                rightFireBallInstance2.GetComponent<Rigidbody>().velocity = magnitude * rightForward2;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagName))
            {
                Debug.Log("Dragon fire ball hit plane");
                OnHit();
            }
        }

        IEnumerator WaitAndDestory(float time)
        {
            yield return new WaitForSeconds(time);
            _animator.SetTrigger("Die");
            Destroy(gameObject, 1f);
        }

        public void OnHit()
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            _animator.SetTrigger("Explode");
            StartCoroutine(WaitAndDestory(1f));
        }
    }
}
