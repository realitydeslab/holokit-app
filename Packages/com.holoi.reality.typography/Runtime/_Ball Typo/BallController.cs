using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace Holoi.Reality.Typography
{
    public class BallController : MonoBehaviour
    {
        public Rigidbody _rigidBody;

        [SerializeField] VisualEffect _vfx;

        [SerializeField] float _radius = 0.3f;

        ARPlane _hitWall;



        void Start()
        {

        }

        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("OnCollisionEnter");

            if (collision.transform.GetComponent<ARPlane>() != null)
            {
                _hitWall = collision.transform.GetComponent<ARPlane>();

#if UNITY_EDITOR
                if (true)
                {
                    Debug.Log(_hitWall.name);
                    _vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                    _vfx.SetVector3("Hit Plane Normal", _hitWall.normal);
                }
#endif
#if UNITY_IOS
                if (_hitWall.alignment == PlaneAlignment.Vertical)
                {
                    Debug.Log(_hitWall.name);
                    _vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                    _vfx.SetVector3("Hit Plane Normal", _hitWall.normal);
                }
#endif
                var hitPos = collision.GetContact(0).point;

                Debug.Log(hitPos);

                _vfx.SetVector3("Hit Position_position", hitPos);

                _vfx.SendEvent("OnExplode");
            }




        }

        public void ClearHitWall()
        {
            _hitWall = null;
            _vfx.SetVector3("Hit Plane Position_position", new Vector3(0, 999, 0));
            _vfx.SetVector3("Hit Plane Normal", new Vector3(0,1,0));
        }
    }
}
