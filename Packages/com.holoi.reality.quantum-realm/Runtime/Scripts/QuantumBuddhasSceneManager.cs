using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;
using Unity.Netcode;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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
        [Header("UI Components")]

        [SerializeField] Button _switchButton;

        [Header("AR Features")]
        [SerializeField] ARRaycastManager _arRaycastManager;
        [SerializeField] ARPlaneManager _arPlaneManager;
        [SerializeField] AROcclusionManager _arOcclusionManager;

        [Header("Placement Settings")]
        [SerializeField] ARRayCastController _arRaycastController;
        [SerializeField] RaycastPlacmentVisualController _raycastVisual;

        [SerializeField] Vector3 _offset = Vector3.zero;

        [SerializeField] GameObject _targetGameObject;

        Transform _centerEye;
        int _amount = 0;
        int _index = 0;
        Animator _vfxAnimator;
        Animator _seatAnimator;

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

            _switchButton.onClick.AddListener(SwitchToNextVFXNetWork);

            _centerEye = HoloKitCamera.Instance.CenterEyePose;
        }

        void Update()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // update hand hook's head position
                var dir = (_handJoint.position - _serverCenterEye.position).normalized;

                _handHookHead.position = _handJoint.position + dir * 0.5f + _serverCenterEye.up*0.2f;
            }

            if(HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Mono)
            {
                _arOcclusionManager.enabled = true;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            }
            else
            {
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
            }
        }

        void SwitchtoNextVFX()
        {
            // do on host:
            Debug.Log($"OnActiveBuddhasChanged: {_index}");

            BuddhasController controller = vfxs[_index].transform.parent.GetComponent<BuddhasController>();

            _vfxAnimator = controller.vfxAnimator;
            _seatAnimator = controller.setaAnimator;

            if (_vfxAnimator != null)
            {
                _vfxAnimator.SetTrigger("Fade Out");
                _seatAnimator.SetTrigger("Fade Out");
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
                else if (i == _index - 1)
                {
                    StartCoroutine(DisableGameObjectAfterTimes(vfxs[i].gameObject, 1.5f));
                }
                else
                {

                }
            }

            _vfxAnimator = vfxs[_index].transform.parent.GetComponent<BuddhasController>().vfxAnimator;
            _seatAnimator = vfxs[_index].transform.parent.GetComponent<BuddhasController>().setaAnimator;

            _vfxAnimator.Rebind();
            _vfxAnimator.Update(0f);

            _seatAnimator.Rebind();
            _seatAnimator.Update(0f);

            // hand vfx:
            TriggerHandVFX();
        }

        public void SwitchToNextVFXNetWork()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // do on host:
                SwitchtoNextVFX();
                // do on client:
                OnBuddhasSwitchedClientRpc(_index - 1);
            }
            else
            {
                // do on client:
                SwitchtoNextVFX();
                // do on host:
                OnBuddhasSwitchedServerRpc(_index - 1);
            }
        }

        public void InitTargetGameObject()
        {
            DisableARRaycastManager();

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
                _arPlaneManager.enabled = false;
                var planeList = FindObjectsOfType<ARPlane>();
                foreach (var plane in planeList)
                {
                    Destroy(plane.gameObject);
                }
            }
            else
            {
                return;
            }
            
        }

        IEnumerator DisableGameObjectAfterTimes(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }

        public void DisableARRaycastManager()
        {
            // if client
            if (HoloKitApp.Instance.IsHost)
            {
                // disble ar ui script:
                _arRaycastController.enabled = false;
                // play die animation
                _raycastVisual.PlayDie();
                // disable go
                StartCoroutine(DisableGameObjectAfterTimes(_arRaycastController.gameObject, 2f));

                OnDisableARRaycastClientRpc();
                return;
            }
            else
            {
                return;
            }
        }

        public void SetPlacementLoadButton(bool state)
        {
            //Debug.Log("SetPlacementLoadButton: " + state);
            if (HoloKitApp.Instance.IsHost)
            {
                _placementLoadButton.gameObject.SetActive(state);
            }
        }

        [ClientRpc]
        public void OnDisableARRaycastClientRpc()
        {
            Debug.Log($"OnDisableARRaycastClientRpc");
            // disble ar ui script:
            _arRaycastController.enabled = false;
            // play die animation
            _raycastVisual.PlayDie();
            // disable go
            StartCoroutine(DisableGameObjectAfterTimes(_arRaycastController.gameObject, 2f));
        }

        //[ClientRpc]
        //public void DisbleARPlaneManagerClientRpc()
        //{
        //    _arRaycastManager.enabled = false;
        //    _arPlaneManager.enabled = false;
        //    var planeList = FindObjectsOfType<ARPlane>();
        //    foreach (var plane in planeList)
        //    {
        //        Destroy(plane.gameObject);
        //    }
        //}

        [ServerRpc(RequireOwnership = false)]
        public void OnBuddhasSwitchedServerRpc(int index)
        {
            if (!IsServer)
            {
                return;
            }

            _index = index;

            Debug.Log($"OnBuddhasSwitchedServerRpc: {_index}");

            BuddhasController controller = vfxs[index].transform.parent.GetComponent<BuddhasController>();

            _vfxAnimator = controller.vfxAnimator;
            _seatAnimator = controller.setaAnimator;

            if (_vfxAnimator != null)
            {
                _vfxAnimator.SetTrigger("Fade Out");
                _seatAnimator.SetTrigger("Fade Out");
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
                else if (i == _index - 1)
                {
                    StartCoroutine(DisableGameObjectAfterTimes(vfxs[i].gameObject, 1.5f));
                }
                else
                {
                }
            }

            _vfxAnimator = vfxs[index].transform.parent.GetComponent<BuddhasController>().vfxAnimator;
            _seatAnimator = vfxs[index].transform.parent.GetComponent<BuddhasController>().setaAnimator;

            _vfxAnimator.Rebind();
            _vfxAnimator.Update(0f);

            _seatAnimator.Rebind();
            _seatAnimator.Update(0f);

            // hand vfx:
            TriggerHandVFX();
        }

        [ClientRpc]
        public void OnBuddhasSwitchedClientRpc(int index)
        {
            if (IsServer)
            {
                return;
            }

            _index = index;

            Debug.Log($"OnActiveBuddhasChangedClientRpc: {_index}");

            BuddhasController controller = vfxs[index].transform.parent.GetComponent<BuddhasController>();

            _vfxAnimator = controller.vfxAnimator;
            _seatAnimator = controller.setaAnimator;

            if (_vfxAnimator != null)
            {
                _vfxAnimator.SetTrigger("Fade Out");
                _seatAnimator.SetTrigger("Fade Out");
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
                else if (i == _index - 1)
                {
                    StartCoroutine(DisableGameObjectAfterTimes(vfxs[i].gameObject, 1.5f));
                }
                else
                {
                }
            }

            _vfxAnimator = vfxs[index].transform.parent.GetComponent<BuddhasController>().vfxAnimator;
            _seatAnimator = vfxs[index].transform.parent.GetComponent<BuddhasController>().setaAnimator;

            _vfxAnimator.Rebind();
            _vfxAnimator.Update(0f);

            _seatAnimator.Rebind();
            _seatAnimator.Update(0f);

            // hand vfx:
            TriggerHandVFX();
        }

        void TriggerHandVFX()
        {
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