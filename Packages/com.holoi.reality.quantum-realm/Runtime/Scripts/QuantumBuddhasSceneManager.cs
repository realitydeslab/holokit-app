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
//using Apple.CoreHaptics;

namespace Holoi.Reality.QuantumRealm
{
    public class QuantumBuddhasSceneManager : RealityManager
    {
        [Header("Buddhas Objects")]
        public List<VisualEffect> vfxs = new List<VisualEffect>();
        [SerializeField] GameObject _objectPrefab;
        [HideInInspector] public BuddhasDisplay BuddhasDisPlay;
        [HideInInspector] public int BuddhasTotalCount = 0;

        [Header("Hand Objects")]
        [SerializeField] Transform _handJoint;
        [SerializeField] Transform _handHookHead;
        [SerializeField] BuddhasHandController _handController;
        [SerializeField] HandObject _ho;

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

        Transform _centerEye;

        int _currentIndex = 0;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void OnEnable()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged += ARFeaturesChangeOnRenderModeChanged;
        }

        private void OnDisable()
        {
            HoloKitCamera.OnHoloKitRenderModeChanged -= ARFeaturesChangeOnRenderModeChanged;
        }

        void Start()
        {
            // server and client set up
            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastManager.enabled = true;
                _arPlaneManager.enabled = true;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = true;

                HoloKitHandTracker.Instance.Active = true;
                HandObject.Instance.IsSyncedHand = false;
                ARRayCastController.Instance.enabled = true;
            }
            else
            {
                _arRaycastManager.enabled = false;
                _arPlaneManager.enabled = false;
                _serverCenterEye.GetComponent<FollowMovementManager>().enabled = false;

                HoloKitHandTracker.Instance.Active = false;
                HandObject.Instance.IsSyncedHand = true;
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

                SyncHandValidStateCLientRpc(_ho.IsValid);
            }
        }

        void ARFeaturesChangeOnRenderModeChanged(HoloKitRenderMode mode)
        {
            if(mode == HoloKitRenderMode.Mono)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
            }
            else
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
            }
        }

        void SetUpSwitchButtonOnCanvas()
        {
            _switchButton.gameObject.SetActive(true);
            _switchButton.onClick.AddListener(SwitchToNextVFXNetWork);

        }

        public void SwitchToNextVFXNetWork()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                // do on host:
                SwitchtoNextVFX();
                // do on client:
                OnBuddhasSwitchedClientRpc(_currentIndex);
            }
            else
            {
                // do on client:
                SwitchtoNextVFX();
                // do on host:
                OnBuddhasSwitchedServerRpc(_currentIndex);
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

        IEnumerator DisableGOAfterTimes(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }

        public void DisableARRaycastManager()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arRaycastController.enabled = false;
                // play die animation
                _raycastVisual.PlayDie();
                // disable go
                StartCoroutine(DisableGOAfterTimes(_arRaycastController.gameObject, 2f));

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
            if (HoloKitApp.Instance.IsHost)
            {
                if (state)
                {
                    _placementLoadButton.gameObject.SetActive(state);
                }
                else
                {
                    if (_placementLoadButton.isActiveAndEnabled)
                    _placementLoadButton.OnDie();
                }

            }
        }

        void SwitchtoNextVFX()
        {
            //Debug.Log($"OnActiveBuddhasChanged: {_currentIndex}");
            BuddhasController controller = BuddhasDisPlay.Buddhas[_currentIndex];
            controller.ExitBuddha();
            BuddhasDisPlay.DisableBuddha(_currentIndex);

            _currentIndex++;
            if (_currentIndex == BuddhasTotalCount) _currentIndex = 0;

            BuddhasDisPlay.EnbaleBuddha(_currentIndex);

            BuddhasController controllerNew = BuddhasDisPlay.Buddhas[_currentIndex];
            controllerNew.InitBuddha();

            // hand vfx:
            _handController.OnExplode();
        }

        [ClientRpc]
        public void OnDisableARRaycastClientRpc()
        {
            // disble ar ui script:
            _arRaycastController.enabled = false;
            // play die animation
            _raycastVisual.PlayDie();
            // disable go
            StartCoroutine(DisableGOAfterTimes(_arRaycastController.gameObject, 2f));
        }

        [ServerRpc(RequireOwnership = false)]
        public void OnBuddhasSwitchedServerRpc(int index)
        {
            if (!IsServer)
            {
                return;
            }

            var lastIndex = index - 1 < 0 ? BuddhasTotalCount - 1 : index - 1;
            _currentIndex = lastIndex;

            BuddhasController controller = BuddhasDisPlay.Buddhas[_currentIndex];
            controller.ExitBuddha();
            BuddhasDisPlay.DisableBuddha(_currentIndex);

            _currentIndex++;
            if (_currentIndex == BuddhasTotalCount) _currentIndex = 0;

            BuddhasDisPlay.EnbaleBuddha(_currentIndex);

            BuddhasController controllerNew = BuddhasDisPlay.Buddhas[_currentIndex];
            controllerNew.InitBuddha();

            // hand vfx:
            _handController.OnExplode();
        }

        [ClientRpc]
        public void OnBuddhasSwitchedClientRpc(int index)
        {
            if (IsServer)
            {
                return;
            }

            var lastIndex = index - 1 < 0 ? BuddhasTotalCount - 1 : index - 1;
            _currentIndex = lastIndex;

            BuddhasController controller = BuddhasDisPlay.Buddhas[_currentIndex];
            controller.ExitBuddha();
            BuddhasDisPlay.DisableBuddha(_currentIndex);

            _currentIndex++;
            if (_currentIndex == BuddhasTotalCount) _currentIndex = 0;

            BuddhasDisPlay.EnbaleBuddha(_currentIndex);

            BuddhasController controllerNew = BuddhasDisPlay.Buddhas[_currentIndex];
            controllerNew.InitBuddha();

            // hand vfx:
            _handController.OnExplode();
        }

        [ClientRpc]
        public void OnBuddhasInitializedClientRpc()
        {

            SetUpSwitchButtonOnCanvas();

            // create main object
            if (HoloKitApp.Instance.IsHost)
            {
                GameObject go = Instantiate(_objectPrefab);

                go.transform.position = new Vector3(_centerEye.position.x, _arRaycastController.transform.position.y, _centerEye.position.z);

                var target = new Vector3(_centerEye.position.x, go.transform.position.y, _centerEye.position.z);

                var eyeHorizentalForward = GetHorizontalForward(_centerEye);

                target = go.transform.position - eyeHorizentalForward;

                go.transform.LookAt(target);

                go.GetComponent<NetworkObject>().Spawn();
            }
            else
            {

            }
        }

        [ClientRpc]
        public void SyncHandValidStateCLientRpc(bool valid)
        {
            if (HoloKitApp.Instance.IsHost)
            {
                return;
            }
            else
            {
                _ho.IsValid = valid;
                //_ho.handAnimator.SetBool("HandValid", valid);
            }
        }
    }
}