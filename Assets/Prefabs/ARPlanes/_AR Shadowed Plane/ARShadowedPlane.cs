using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloKit;

public class ARShadowedPlane : MonoBehaviour
{
    Material _shadowedMat;

    private void OnEnable()
    {
        HoloKitCamera.OnHoloKitRenderModeChanged += OnRenderModeChanged;
    }

    private void OnDisable()
    {
        HoloKitCamera.OnHoloKitRenderModeChanged -= OnRenderModeChanged;
    }

    void Start()
    {
        _shadowedMat = GetComponent<MeshRenderer>().material;
    }

    void OnRenderModeChanged(HoloKitRenderMode mode)
    {
        if (mode == HoloKitRenderMode.Mono)
        {
                _shadowedMat.SetInt("_IsSTAR", 0);
        }
        else
        {
            _shadowedMat.SetInt("_IsSTAR", 1);
        }
    }
}
