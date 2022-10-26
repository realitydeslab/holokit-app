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

        public GameObject VisualSampleObject;
        public Animator Animator;
        int count = 0;

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
            if(HoloKitHandTracker.Instance.Valid == true)
            IsValid = HoloKitHandTracker.Instance.Valid;
#endif

            Animator.SetBool("HandValid", IsValid);

        }
    }
}