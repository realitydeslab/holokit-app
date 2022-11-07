using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;
using Unity.AI.Navigation;
using UnityEngine.XR.ARFoundation;

namespace Holoi.Reality.Dragon
{
    public class DragonRealitySceneManager : MonoBehaviour
    {
        [Header("AR UI Components")]
        [SerializeField] HoverableStartButton _placementLoadButton;
        [Header("UI Components")]
        [SerializeField] GameObject _switchButton;
        [SerializeField] GameObject _tipsText;
        [Header("AR Mesh Baker")]
        [SerializeField] NavigationBaker _navigationBaker;

        [Header("Placement Settings")]
        [SerializeField] ARRayCastController _arRaycastController;

        [SerializeField] Vector3 _offset = Vector3.zero;

        [Header("Game Phase")]
        [SerializeField] GameObject _arRaycast;
        [SerializeField] GameObject _targetGameObject;

        void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {

            }
            else
            {
                _switchButton.SetActive(false);
            }
        }

        void Update()
        {

        }

        public void BuildNavigation()
        {
            _tipsText.SetActive(false);

            // disable ar meshing
            var arMeshManager = FindObjectOfType<ARMeshManager>();
            arMeshManager.enabled = false;

            // add all mesh here
            var surfaceList = FindObjectsOfType<NavMeshSurface>();
            _navigationBaker.surfaces.Clear();

            foreach (var surface in surfaceList)
            {
                _navigationBaker.surfaces.Add(surface);
                //surface.GetComponent<MeshRenderer>().enabled = false; // make it invisible
            }

            Debug.Log($"Build with {surfaceList.Length} surfaces ");

            _navigationBaker.BuildNavMesh();

            _arRaycast.SetActive(true);
        }

        public void InitTargetGameObject()
        {
            _targetGameObject.SetActive(true);

            _targetGameObject.transform.position = _arRaycastController.transform.position + _offset;

            //var playerPos = HoloKitCamera.Instance.CenterEyePose.position;

            //var targetPos = new Vector3(playerPos.x, _targetGameObject.transform.position.y, playerPos.z);

            //_targetGameObject.transform.LookAt(targetPos);

            //_targetGameObject.transform.parent = transform;
        }

        public void DisableARRaycast()
        {
            _arRaycastController.enabled = false;
        }

        public void SetPlacementLoadButton(bool state)
        {
            //Debug.Log("SetPlacementLoadButton: " + state);
            _placementLoadButton.gameObject.SetActive(state);
        }
    }
}