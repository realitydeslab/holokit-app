using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.Typography
{
    public class StickerManRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementIndicator _arPlacementIndicator;

        [SerializeField] private HoverableStartButton _hoverableStartButton;

        [Header("Hand")]
        public HoverObject HostHandPose;

        [SerializeField] private GameObject _hostHandVisual;

        [Header("The Sculpture")]
        [SerializeField] private GameObject _sculpturePrefab;

        private readonly NetworkVariable<bool> _isHostHandValid = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arOcclusionManager.enabled = true;
                _arRaycastManager.enabled = true;
                _arPlaneManager.enabled = true;
                HoloKitHandTracker.OnHandValidityChanged += OnHandValidityChanged;
                HoloKitHandTracker.Instance.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_hoverableStartButton.gameObject);
                Destroy(HostHandPose.GetComponent<FollowTargetController>());
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isHostHandValid.OnValueChanged += OnIsHostHandValidValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isHostHandValid.OnValueChanged -= OnIsHostHandValidValueChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitHandTracker.OnHandValidityChanged -= OnHandValidityChanged;
            }
        }

        public void SpawnSculpture()
        {
            var hitPoint = _arPlacementIndicator.HitPoint;
            var sculpture = Instantiate(_sculpturePrefab, hitPoint.position, hitPoint.rotation);
            sculpture.GetComponent<NetworkObject>().Spawn();
        }

        private void OnHandValidityChanged(bool isValid)
        {
            _isHostHandValid.Value = isValid;
        }

        private void OnIsHostHandValidValueChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                HostHandPose.IsActive = true;
                _hostHandVisual.SetActive(true);
                return;
            }

            if (oldValue && !newValue)
            {
                HostHandPose.IsActive = false;
                _hostHandVisual.SetActive(false);
                return;
            }
        }

        //public static Vector3 GetHorizontalForward(Transform transform)
        //{
        //    return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        //}

        //public void InitializeRealityObject()
        //{
        //    Debug.Log("InitializeRealityObject");
        //    _prefabInstance = Instantiate(_sculpturePrefab);

        //    _prefabInstance.transform.position = _arRaycastController.transform.position;

        //    //var eyeHorizentalForward = GetHorizontalForward(_centerEye);

        //    var lookAtPoint = _prefabInstance.transform.position;

        //    _prefabInstance.transform.LookAt(lookAtPoint);

        //    _prefabInstance.GetComponent<NetworkObject>().Spawn();

        //    SwitchARPlaneToShadowed();
        //}

        //IEnumerator DisableGameObjectAfterTimes(GameObject go, float time)
        //{
        //    yield return new WaitForSeconds(time);
        //    go.SetActive(false);
        //}

        //public void DisableARPlaneManager()
        //{
        //    _arPlaneManager.enabled = false;
        //    _arPlaneManager.enabled = false;
        //    var planeList = FindObjectsOfType<ARPlane>();
        //    foreach (var plane in planeList)
        //    {
        //        Destroy(plane.gameObject);
        //    }
        //}

        //public void DisableARRaycastManager()
        //{
        //    // disble ar ui script:
        //    _arRaycastController.enabled = false;
        //    // play die animation
        //    _raycastVisualController.PlayDie();
        //    // disable go
        //    StartCoroutine(DisableGameObjectAfterTimes(_arRaycastController.gameObject, 2f));
        //}

        //public void OnInteractionTriggered()
        //{
        //    TriggerHandVFX();
        //}

        //void TriggerHandVFX()
        //{
        //    _handLoadedVFXParent.gameObject.SetActive(true);
        //    StartCoroutine(DisableGameObjectAfterTimes(_handLoadedVFXParent.gameObject, 2.5f));
        //}

        //public void SetPlacementLoadButton(bool state)
        //{
        //    //Debug.Log("SetPlacementLoadButton: " + state);
        //    if (HoloKitApp.Instance.IsHost)
        //    {
        //        _placementLoadButton.gameObject.SetActive(state);
        //    }
        //}


        //[ClientRpc]
        //public void SyncHandValidStateCLientRpc(bool valid)
        //{
        //    if (HoloKitApp.Instance.IsHost)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (_ho.IsSyncedHand)
        //        {
        //            _ho.IsValid = valid;
        //        }
        //    }
        //}

        //public void SwitchARPlaneToShadowed()
        //{
        //    // on host:

        //    _arPlaneManager.planePrefab = _arShadowedPlane;
        //    _arPlaneManager.enabled = false; // do not update planes

        //    //var planeList = FindObjectsOfType<ARPlane>();
        //    //foreach (var plane in planeList)
        //    //{
        //    //    plane.GetComponent<MeshRenderer>().material = _arShadowedPlaneMat;
        //    //}

        //    // on client: 
        //    SwitchARPlaneToShadowedCLientRpc();
        //}

        //[ClientRpc]
        //public void SwitchARPlaneToShadowedCLientRpc()
        //{
        //    _arPlaneManager.planePrefab = _arShadowedPlane;
        //    _arPlaneManager.enabled = false;

        //}
    }
}