using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.Typography
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] VisualEffect _vfx;
        [SerializeField] bool _usingPrefab;
        [SerializeField] GameObject _textPrefab;
        [SerializeField] float _radius = 0.3f;



        void Start()
        {
        
        }

        void Update()
        {
        
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("OnCollisionEnter");

            if (_usingPrefab)
            {
                OnExplodePrefabMode();
            }
            else
            {
                var hitPos = collision.transform.position;
                _vfx.SetVector3("Hit Position_position", hitPos);
                _vfx.SendEvent("OnExplode");
            }
        }

        void OnExplodePrefabMode()
        {
            for (int i = 0; i < 1000; i++)
            {
                var go = Instantiate(_textPrefab, transform);
                var offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                offset = offset.normalized;
                offset *= _radius;
                go.transform.position = transform.position + offset;
                var direction = go.transform.position - transform.position;
                direction = direction.normalized;
                go.GetComponent<Rigidbody>().velocity = direction * 3;
            }
        }
    }
}
