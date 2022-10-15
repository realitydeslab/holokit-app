using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;
using Unity.Netcode;
using HoloKit;
using UnityEngine.VFX;

namespace Holoi.Reality.TypoGraphy
{
    public class TextRainManager : MonoBehaviour
    {
        public GameObject _rainPrefab;
        public Transform Root;
        BoneController _bone;
        VisualEffect _vfxCloud;
        VisualEffect _vfxRain;

        bool _isValid = false;

        void Start()
        {
            _vfxCloud = GetComponent<VisualEffect>();
            _vfxRain = transform.GetChild(0).GetComponent<VisualEffect>();
        }

        void Update()
        {
            if (FindObjectOfType<BoneController>() != null)
            {
                _isValid = true;
                _bone = FindObjectOfType<BoneController>();
            }

            if (_isValid)
            {
                _vfxCloud.enabled = true;
                _vfxRain.enabled = true;

                GetComponent<FollowMovementManager>().FollowTarget = _bone.skeletonRoot;
                GetComponent<FollowMovementManager>().enabled = true;
                _vfxRain.SetVector3("Root Position_position", _bone.skeletonRoot.position);
            }
        }
    }
}
