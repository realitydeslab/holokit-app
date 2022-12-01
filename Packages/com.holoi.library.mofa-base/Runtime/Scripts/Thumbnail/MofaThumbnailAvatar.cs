using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [RequireComponent(typeof(Animator))]
    public class MofaThumbnailAvatar : MonoBehaviour
    {
        [SerializeField] Transform _parent;

        [SerializeField] Vector2 _avatarVelocity;

        [SerializeField] bool _autoFire = true;

        [SerializeField] GameObject _boltPrefab;

        [SerializeField] float _attackPreset = 0f;

        [SerializeField] float _attackInterval = 3f;

        Animator _avatarAnimator;

        private readonly Queue<GameObject> _pool = new();

        void Start()
        {
            if (_avatarAnimator == null)
            {
                _avatarAnimator = GetComponent<Animator>();
            }
            else
            {
                Debug.Log("No Animator Assign to this Avatar");
            }

            if (_autoFire)
            {
                if (_boltPrefab)
                {
                    StartCoroutine(WaitAndBegin(_attackPreset, _attackInterval));
                }
                else
                {
                    Debug.Log("missing fire bolt prefab.");
                }
            }

            _avatarAnimator.SetFloat("Velocity Z", _avatarVelocity.x);
            _avatarAnimator.SetFloat("Velocity X", _avatarVelocity.y);
        }

        //void Update()
        //{
        //    _avatarAnimator.SetFloat("Velocity Z", _avatarVelocity.x);
        //    _avatarAnimator.SetFloat("Velocity X", _avatarVelocity.y);
        //}

        IEnumerator WaitAndBegin(float time, float interval)
        {
            yield return new WaitForSecondsRealtime(time);
            StartCoroutine(WaitAndShoot(interval));
        }

        IEnumerator WaitAndShoot(float time)
        {
            _avatarAnimator.SetTrigger("Attack A");
            yield return new WaitForSecondsRealtime(0.25f);
            ShootBolt();
            yield return new WaitForSecondsRealtime(time - 0.25f);
            StartCoroutine(WaitAndShoot(_attackInterval));
        }

        private void AddObjectToQueue(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var bolt = Instantiate(_boltPrefab, _parent);
                bolt.GetComponent<MofaThumbnailBoltMagic>().SetPool(this);
                bolt.SetActive(false);
                _pool.Enqueue(bolt);
            }
        }

        public void ReturnObjectToQueue(GameObject go)
        {
            go.SetActive(false);
            _pool.Enqueue(go);
        }

        void ShootBolt()
        {
            if (_pool.Count == 0)
            {
                AddObjectToQueue(1);
            }
            var bolt = _pool.Dequeue();
            bolt.SetActive(true);
            // Rotation
            bolt.transform.LookAt(transform.forward * 5f);
            // Position
            bolt.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1f;
            // Add velocity
            bolt.GetComponent<Rigidbody>().velocity = transform.forward * 3f;
        }
    }
}
