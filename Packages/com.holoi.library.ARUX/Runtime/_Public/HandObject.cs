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

        public Animator handAnimator;

        public bool IsSyncedHand = false;

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

        private void Start()
        {

        }

        private void Update()
        {

            if (HoloKitUtils.IsRuntime)
            {
                if (!IsSyncedHand)
                {
                    IsValid = HoloKitHandTracker.Instance.Valid;
                } 
            }
            else
            {
                // editor mode:
            }

            if(handAnimator) handAnimator.SetBool("HandValid", IsValid);
        }
    }
}