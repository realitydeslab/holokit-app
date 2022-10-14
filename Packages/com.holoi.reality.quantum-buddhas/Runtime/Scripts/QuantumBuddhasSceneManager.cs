using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;
using Holoi.Library.ARUX;
using HoloKit;
using Unity.Netcode;

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
        [SerializeField] RaycastPlacmentVisualController _RaycastVisual;

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
            if (!HoloKitApp.Instance.IsHost)
            {
                return;
            }

            var realityManager = HoloKitApp.Instance.RealityManager as QuantumBuddhasRealityManager;

            realityManager.OnActiveBuddhasSwitchClientRpc(_index);
        }

        public void SwitchToNextVFXClientRpc()
        {
            _animator = _vfxs[_index].GetComponent<Animator>();
            if (_animator !=null)
            {
                _animator.SetTrigger("Fade Out");
                _animator.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fade Out");
            }

            _index++;

            if (_index == _vfxs.Count) _index = 0;

            // disbale other vfx after play animation:
            for (int i = 0; i < _vfxs.Count; i++)
            {
                if(i == _index)
                {
                    _vfxs[_index].gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"disable with index: {i}");
                    StartCoroutine(DisableAfterTimes(_vfxs[i].gameObject, 1.5f));
                }
            }
            _animator = _vfxs[_index].GetComponent<Animator>();
            _animator.Rebind();
            _animator.Update(0f);
            // vfx:
            _handLoadedVFXParent.gameObject.SetActive(true);
            StartCoroutine(DisableAfterTimes(_handLoadedVFXParent.gameObject, 2.5f));
        }

        public void InitTargetGameObject()
        {
            var realityManager = HoloKitApp.Instance.RealityManager as QuantumBuddhasRealityManager;

            realityManager.OnBuddhasEnabledClientRpc(); // all client run this function
        }

        public void InitTargetGameObjectClient()
        {
            _targetGameObject.SetActive(true);

            _targetGameObject.transform.position = _arRaycastController.transform.position + _offset;
        }

        public void DisableARRaycast()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                return;
            }

            var realityManager = HoloKitApp.Instance.RealityManager as QuantumBuddhasRealityManager;

            realityManager.OnDisableARRaycastClientRpc();
        }

        public void DisableARRaycastClientRpc()
        {
            // diable function:
            _arRaycastController.enabled = false;
            // play die animation
            _RaycastVisual.PlayDie();
            // disable go
            StartCoroutine(DisableAfterTimes(_arRaycastController.gameObject,2f));
        }

        public void SetPlacementLoadButton(bool state)
        {
            //Debug.Log("SetPlacementLoadButton: " + state);
            _placementLoadButton.gameObject.SetActive(state);
        }

        IEnumerator DisableAfterTimes(GameObject go,float time)
        {
            yield return new WaitForSeconds(time);
            Debug.Log($"Set {go.name} to disable.");
            go.SetActive(false);
        }
    }
}