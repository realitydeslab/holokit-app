using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkedCube : NetworkBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            Transform cameraTransform = Camera.main.transform;
            transform.SetPositionAndRotation(cameraTransform.position, cameraTransform.rotation);
        }
    }
}
