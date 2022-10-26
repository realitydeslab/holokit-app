using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class HandObject : MonoBehaviour
    {
        private static HandObject _instance;

        public static HandObject Instance { get { return _instance; } }

        public bool IsValid = false;

        [SerializeField] GameObject _visualSampleObject;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            // do nothing
#else
            IsValid = HoloKitHandTracker.Instance.Valid;
#endif
            if (IsValid)
            {
                _visualSampleObject.SetActive(true);
            }
            else
            {
                _visualSampleObject.SetActive(false);
            }
        }
    }
}