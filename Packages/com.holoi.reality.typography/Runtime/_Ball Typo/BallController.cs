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

        [SerializeField] List<VisualEffect> _vfxs;

        [SerializeField] float _radius = 0.3f;

        ARPlane _hitWall;

        bool _isCollisionTriggeredFirstTime = true;

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
            if (HoloKitApp.Instance.IsHost)
            {
                GetComponent<FollowMovementManager>().enabled = true;
            }
            else
            {
                GetComponent<FollowMovementManager>().enabled = false;

            }
        }

        void Update()
        {

        }

        public void OnHandsUp()
        {
            _isCollisionTriggeredFirstTime = true;
            foreach(var vfx in _vfxs)
            {
                vfx.SetBool("Is Alive", true);
            }
            
            if (HoloKitApp.Instance.IsHost)
                GetComponent<FollowMovementManager>().enabled = true;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.useGravity = false;
        }

        public void OnShoot(Vector3 direction)
        {
            if (HoloKitApp.Instance.IsHost)
                GetComponent<FollowMovementManager>().enabled = false;
            _rigidBody.useGravity = true;
            _rigidBody.velocity = direction * 12;

            GetComponent<BallController>().ClearHitWall(); // clear hit wall every time you shoot to avoid the unexpected hit wall.
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("OnCollisionEnter");

            if (HoloKitApp.Instance.IsHost && _isCollisionTriggeredFirstTime)
            {
                if (collision.transform.GetComponent<ARPlane>() != null)
                {
                    _hitWall = collision.transform.GetComponent<ARPlane>();

#if UNITY_EDITOR
                    if (true)
                    {
                        var hitPos = collision.GetContact(0).point;
                        foreach (var vfx in _vfxs)
                        {
                            vfx.SetBool("Is Alive", false);
                            vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                            vfx.SetVector3("Hit Plane Normal", _hitWall.normal);
                            vfx.SetVector3("Hit Position_position", hitPos);
                            vfx.SendEvent("OnExplode");
                        }

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
                        var hitPos = collision.GetContact(0).point;

                        foreach (var vfx in _vfxs)
                        {
                            vfx.SetBool("Is Alive", false);
                            vfx.SetVector3("Hit Plane Position_position", _hitWall.transform.position);
                            vfx.SetVector3("Hit Plane Normal", _hitWall.normal);

                            vfx.SetVector3("Hit Position_position", hitPos);

                            vfx.SendEvent("OnExplode");
                        }

                        Debug.Log("Call OnCollisionEnterVFXChangesClientRpc");

                        OnCollisionEnterVFXChangesClientRpc(
                            _hitWall.transform.position,
                            _hitWall.normal,
                            hitPos
                            );
                    }
#endif
                }
                _isCollisionTriggeredFirstTime = false;
            }
            else
            {

            }
        }

        public void ClearHitWall()
        {
            _hitWall = null;
            foreach (var vfx in _vfxs)
            {
                vfx.SetVector3("Hit Plane Position_position", new Vector3(0, 999, 0));
                vfx.SetVector3("Hit Plane Normal", new Vector3(0, 1, 0));
            }
        }

        [ClientRpc]
        void OnCollisionEnterVFXChangesClientRpc(Vector3 hitWallPos, Vector3 hitWallNormal, Vector3 hitPos)
        {
            Debug.Log("OnCollisionEnterVFXChangesClientRpc");

            foreach (var vfx in _vfxs)
            {
                vfx.SetBool("Is Alive", false);
                vfx.SetVector3("Hit Plane Position_position", hitWallPos);

                vfx.SetVector3("Hit Plane Normal", hitWallNormal);

                vfx.SetVector3("Hit Position_position", hitPos);

                vfx.SendEvent("OnExplode");
            }
        }
    }
}
