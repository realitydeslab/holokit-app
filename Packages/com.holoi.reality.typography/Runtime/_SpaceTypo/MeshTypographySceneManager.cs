using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using HoloKit;
using UnityEngine.XR.ARFoundation.Samples;

public class MeshTypographySceneManager : MonoBehaviour
{

    private void Awake()
    {
        var _videoEnhancementMode = VideoEnhancementMode.HighResWithHDR;
        HoloKitARSessionControllerAPI.SetVideoEnhancementMode(_videoEnhancementMode); 
    }
    void Start()
    {
        //FindObjectOfType<ARPlaneManager>(true).enabled = true;
        //FindObjectOfType<ARMeshManager>(true).enabled = true;
        //FindObjectOfType<ToggleMeshClassification>(true).enabled = true;
    }

    public void OnMeshingDone()
    {
        FindObjectOfType<ARPlaneManager>(true).enabled = false;
        FindObjectOfType<ARMeshManager>(true).enabled = false;
        FindObjectOfType<ToggleMeshClassification>(true).enabled = false;
    }
}
