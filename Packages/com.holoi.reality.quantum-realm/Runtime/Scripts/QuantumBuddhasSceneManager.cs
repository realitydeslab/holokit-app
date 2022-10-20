using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;
using Unity.Netcode;
using UnityEngine.XR.ARFoundation;

namespace Holoi.Reality.QuantumRealm
{
    public class QuantumBuddhasSceneManager : RealityManager
    {
        [Header("Buddhas Objects")]
        public List<VisualEffect> vfxs = new List<VisualEffect>();

        [Header("Hand Objects")]
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _handHookHead;
        [SerializeField] Transform _handLoadedVFXParent;

        [Header("AR NetWork Base Objects")]
        [SerializeField] Transform _serverCenterEye;


        [Header("AR UI Components")]
        [SerializeField] LoadButtonController _placementLoadButton;

        [Header("AR Features")]
        [SerializeField] ARRaycastManager _arRaycastManager;
        [SerializeField] ARPlaneManager _arPlaneManager;
        [SerializeField] AROcclusionManager _arOcclusioManager;

        [Header("Placement Settings")]
        [SerializeField] ARRayCastController _arRaycastController;
        [SerializeField] RaycastPlacmentVisualController _RaycastVisual;

        [SerializeField] Vector3 _offset = Vector3.zero;

        [SerializeField] GameObject _targetGameObject;

        Transform _centerEye;
        int _amount = 0;
        int _index = 0;
        Animator _animator;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        void Start()
        {
            _amount = vfxs.Count;

            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastManager.enabled = true;
                _arPlaneManager.enabled = true;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = true;

                HoloKitHandTracker.Instance.Active = true;
                HandObject.Instance.enabled = true;
                ARRayCastController.Instance.enabled = true;
            }
            else
            {
                _arRaycastManager.enabled = false;
                _arPlaneManager.enabled = false;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = false;

                HoloKitHandTracker.Instance.Active = false;
                HandObject.Instance.enabled = false;
                ARRayCastController.Instance.enabled = false;
            }

            _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // update hand hook's head position
                var dir = (_handJoint.position - _serverCenterEye.position).normalized;

                _handHookHead.position = _handJoint.position + dir * 0.5f;
            }

            if(HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono)
            {
                _arOcclusioManager.enabled = true;
            }
            else
            {
                _arOcclusioManager.enabled = false;
            }
        }

        public void SwitchToNextVFX()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                return;
            }

            OnBuddhasSwitchedClientRpc(_index);
        }

        public void InitTargetGameObject()
        {
            DisableARRaycastVisual();
            DisableARPlaneManager();

            OnBuddhasInitializedClientRpc(); // all client run this function
        }

        public static Vector3 GetHorizontalForward(Transform transform)
        {
            return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        }

        public void DisableARPlaneManager()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = false;
            }
            
        }

        public void DisableARRaycastVisual()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                return;
            }

            OnDisableARRaycastClientRpc();
        }

        public void SetPlacementLoadButton(bool state)
        {
            //Debug.Log("SetPlacementLoadButton: " + state);
            if (HoloKitApp.Instance.IsHost)
            {
                _placementLoadButton.gameObject.SetActive(state);
            }
        }

        IEnumerator DisableGameObjectAfterTimes(GameObject go,float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }

        public void DisbleARRaycastManager()
        {
            _arRaycastManager.enabled = false;
            _arPlaneManager.enabled = false;
            var planeList = FindObjectsOfType<ARPlane>();
            foreach (var plane in planeList)
            {
                Destroy(plane.gameObject);
            }
        }

        [ClientRpc]
        public void OnDisableARRaycastClientRpc()
        {
            Debug.Log($"OnDisableARRaycastClientRpc");

            // disble ar ui script:
            _arRaycastController.enabled = false;
            // play die animation
            _RaycastVisual.PlayDie();
            // disable go
            StartCoroutine(DisableGameObjectAfterTimes(_arRaycastController.gameObject, 2f));

        }

        [ClientRpc]
        public void OnBuddhasSwitchedClientRpc(int index)
        {
            Debug.Log($"OnActiveBuddhasChangedClientRpc: {index}");

            _animator = vfxs[_index].GetComponent<Animator>();

            if (_animator != null)
            {
                _animator.SetTrigger("Fade Out");
                _animator.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fade Out");
            }

            _index++;

            if (_index == vfxs.Count) _index = 0;

            // disbale other vfx after play animation:
            for (int i = 0; i < vfxs.Count; i++)
            {
                if (i == _index)
                {
                    vfxs[_index].gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"disable with index: {i}");
                    StartCoroutine(DisableGameObjectAfterTimes(vfxs[i].gameObject, 1.5f));
                }
            }
            _animator = vfxs[_index].GetComponent<Animator>();
            _animator.Rebind();
            _animator.Update(0f);
            // vfx:
            _handLoadedVFXParent.gameObject.SetActive(true);
            StartCoroutine(DisableGameObjectAfterTimes(_handLoadedVFXParent.gameObject, 2.5f));
        }

        [ClientRpc]
        public void OnBuddhasInitializedClientRpc()
        {
            Debug.Log($"OnBuddhasEnabledClientRpc");

            // create main object
            if (HoloKitApp.Instance.IsHost)
            {
                GameObject go = Instantiate(_targetGameObject);

                go.transform.position = new Vector3(_centerEye.position.x, _arRaycastController.transform.position.y, _centerEye.position.z);

                var target = new Vector3(_centerEye.position.x, go.transform.position.y, _centerEye.position.z);

                var eyeHorizentalForward = GetHorizontalForward(_centerEye);

                target = go.transform.position - eyeHorizentalForward;

                go.transform.LookAt(target);

                go.GetComponent<NetworkObject>().Spawn();

            }
        }
    }
}