using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Holoi.Library.ARUX;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;



namespace Holoi.Reality.Typography
{
    public class BallController : NetworkBehaviour
    {
        public Rigidbody _rigidBody;

        [SerializeField] VisualEffect _vfx;

        [SerializeField] float _radius = 0.3f;

        ARPlane _hitWall;

        private NetworkVariable<Vector3> _hitWalPos = new NetworkVariable<Vector3>();

        private NetworkVariable<Vector3> _hitWalNormal = new NetworkVariable<Vector3>();

        private NetworkVariable<Vector3> _hitPos = new NetworkVariable<Vector3>();

        private NetworkVariable<float> _floorHeight = new NetworkVariable<float>();

        private NetworkVariable<bool> _isAlive = new NetworkVariable<bool>();

        public override void OnNetworkSpawn()
        {
            Debug.Log("this net work run");
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void OnHandsUp()
        {
            _vfx.SetBool("Is Alive", true);
            GetComponent<FollowMovementManager>().enabled = true;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.useGravity = false;
        }

        public void OnShoot(Vector3 direction)
        {
            GetComponent<FollowMovementManager>().enabled = false;
            _rigidBody.useGravity = true;
            _rigidBody.velocity = direction * 3;

            GetComponent<BallController>().ClearHitWall(); // clear hit wall every time you shoot to avoid the unexpected hit wall.
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("OnCollisionEnter");

            if (HoloKitApp.Instance.IsHost)
            {
                if (collision.transform.GetComponent<ARPlane>() != null)
                {
                    _hitWall = collision.transform.GetComponent<ARPlane>();

#if UNITY_EDITOR
                    if (true)
                    {
                        _vfx.SetBool("Is Alive", false);
                        _vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                        _vfx.SetVector3("Hit Plane Normal", _hitWall.normal);

                        var hitPos = collision.GetContact(0).point;

                        _vfx.SetVector3("Hit Position_position", hitPos);

                        _vfx.SendEvent("OnExplode");

                        OnCollisionEnterVFXChangesClientRpc(
                            _hitWall.transform.position,
                            _hitWall.normal,
                            hitPos
                            );
                    }
#endif
#if UNITY_IOS
                    if (_hitWall.alignment == PlaneAlignment.Vertical)
                    {
                        _vfx.SetBool("Is Alive", false);
                        _vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                        _vfx.SetVector3("Hit Plane Normal", _hitWall.normal);

                        var hitPos = collision.GetContact(0).point;

                        _vfx.SetVector3("Hit Position_position", hitPos);

                        _vfx.SendEvent("OnExplode");

                        Debug.Log("Call OnCollisionEnterVFXChangesClientRpc");

                        OnCollisionEnterVFXChangesClientRpc(
                            _hitWall.transform.position,
                            _hitWall.normal,
                            hitPos
                            );
                    }
#endif
                }
            }
            else
            {

            }
        }

        public void ClearHitWall()
        {
            _hitWall = null;
            _vfx.SetVector3("Hit Plane Position_position", new Vector3(0, 999, 0));
            _vfx.SetVector3("Hit Plane Normal", new Vector3(0, 1, 0));
        }

        [ClientRpc]
        void OnCollisionEnterVFXChangesClientRpc(Vector3 hitWallPos, Vector3 hitWallNormal, Vector3 hitPos)
        {
            Debug.Log("OnCollisionEnterVFXChangesClientRpc");

            _vfx.SetBool("Is Alive", false);
            _vfx.SetVector3("Hit Plane Position_position", hitWallPos);

            _vfx.SetVector3("Hit Plane Normal", hitWallNormal);

            _vfx.SetVector3("Hit Position_position", hitPos);

            _vfx.SendEvent("OnExplode");
        }
    }
}
