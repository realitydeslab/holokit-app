using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Controller;
using MalbersAnimations.Utilities;
using HoloKit;

public class TheDragonController : MonoBehaviour
{
    [SerializeField] private Aim _aim;

    private void Start()
    {
        _aim.MainCamera = HoloKitCamera.Instance.CenterEyePose;
    }
}
