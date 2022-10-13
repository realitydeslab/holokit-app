using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.QuantumBuddhas
{
    public class QuantumBuddhasSceneManager : MonoBehaviour
    {
        [SerializeField] List<VisualEffect> _vfxs = new List<VisualEffect>();
        [SerializeField] Transform _handLoadedVFXParent;

        [Header("AR UI Components")]
        [SerializeField] LoadButtonController _placementLoadButton;

        [Header("Placement Settings")]
        [SerializeField] ARRayCastController _arRaycastController;

        [SerializeField] Vector3 _offset = Vector3.zero;

        [SerializeField] GameObject _targetGameObject;

        Transform _centerEye;
        int _amount = 0;
        int _index = 0;
        Animator _animator;

        void Start()
        {
            _amount = _vfxs.Count;
        }

        void Update()
        {
        
        }

        public void SwitchToNextVFX()
        {
            if(_animator !=null)
            {
                _animator.SetTrigger("Fade Out");
                _animator.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fade Out");
            }

            _index++;
            if (_index == _vfxs.Count) _index = 0;
            // disbale all vfx
            foreach (var vfx in _vfxs)
            {
                vfx.gameObject.SetActive(false);
            }
            // enable the slected
            _vfxs[_index].gameObject.SetActive(true);
            _animator = _vfxs[_index].GetComponent<Animator>();
            //_animator.Play("Fade in", -1, 0f); // reset animator
            _animator.Rebind();
            _animator.Update(0f);
            // vfx:
            _handLoadedVFXParent.gameObject.SetActive(true);
            StartCoroutine(SetVFXDisableAfterTimes());
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
            Debug.Log("SetPlacementLoadButton: " + state);
            _placementLoadButton.gameObject.SetActive(state);
        }

        IEnumerator SetVFXDisableAfterTimes()
        {
            yield return new WaitForSeconds(2.5f);
            _handLoadedVFXParent.gameObject.SetActive(false);
        }
    }
}