using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKitAppNativePlugin;
using System;

public class MPCSessionController : MonoBehaviour
{
    private void Awake()
    {
        MPCSessionControllerAPI.RegisterDelegates();
        MPCSessionControllerAPI.OnBrowserFoundCorrectPeer += OnBrowserFoundCorrectPeer;
        MPCSessionControllerAPI.OnReceivedSessionCode += OnReceivedSessionCode;
    }

    private void OnBrowserFoundCorrectPeer(string peerName)
    {
        
    }

    private void OnReceivedSessionCode(string sessionCode, string peerName)
    {
        App.Instance.JoinReality(sessionCode);
    }
}
