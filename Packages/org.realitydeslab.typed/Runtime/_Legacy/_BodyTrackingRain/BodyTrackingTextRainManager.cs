using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HoloKit;
using UnityEngine.VFX;
using RealityDesignLab.Library.HoloKitApp;
using RealityDesignLab.Library.ARUX;

namespace Holoi.Reality.Typography
{
    // public class BodyTrackingTextRainManager : MonoBehaviour
    // {
        
    //     Transform _head;
    //     Transform _rightHand;
    //     Transform _leftHand;

    //     BoneController _bone;

    //     VisualEffect _vfxCloud;

    //     [Header("vfx rain")]
    //     VisualEffect _vfxRain;

    //     bool _isValid = false;

    //     void Start()
    //     {
    //         _vfxCloud = GetComponent<VisualEffect>();
    //         _vfxRain = transform.GetChild(0).GetComponent<VisualEffect>();
    //     }

    //     void Update()
    //     {
    //         if (FindObjectOfType<BodyTrackingRealityManager>().IsBodyValid)
    //         {
    //             _isValid = true;
    //             _bone = FindObjectOfType<BoneController>();
    //         }

    //         if (_isValid)
    //         {
    //             _vfxCloud.enabled = true;
    //             _vfxRain.enabled = true;

    //             GetComponent<FollowMovementManager>().FollowTarget = _bone.skeletonRoot;
    //             GetComponent<FollowMovementManager>().enabled = true;
    //             _vfxRain.SetVector3("Head Position_position", _bone.SkeletonNeck1.position);
    //             _vfxRain.SetVector3("Chest Position_position", _bone.SkeletonChest.position);
    //             _vfxRain.SetVector3("RH Position_position", _bone.SkeletonRightHand.position);
    //             _vfxRain.SetVector3("LH Position_position", _bone.SkeletonLeftHand.position);
    //             _vfxRain.SetVector3("Plane Position_position", FindObjectOfType<ARRayCastController>().transform.position);
    //         }
    //     }
    // }
}
