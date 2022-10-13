using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.Dragon
{
    public class DragonRealitySceneManager : MonoBehaviour
    {
        [Header("AR UI Components")]
        [SerializeField] LoadButtonController _placementLoadButton;

        [Header("Placement Settings")]
        [SerializeField] ARRayCastController _arRaycastController;

        [SerializeField] Vector3 _offset = Vector3.zero;

        [SerializeField] GameObject _targetGameObject;

        Transform _centerEye;
        Animator _animator;

        void Start()
        {
        }

        void Update()
        {

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