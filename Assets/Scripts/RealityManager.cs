using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HoloKit;

public abstract class RealityManager : NetworkBehaviour
{
    [SerializeField] private Vector3 _cameraToImageOffset;

    [Networked]
    public Vector3 PosePosition { get; set; }

    [Networked]
    public float HoriontalRotation { get; set; }

    public override void Spawned()
    {
        App.Instance.SetRealityManager(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && HoloKitCamera.Instance != null)
        {
            GameObject go = new GameObject();
            go.transform.SetPositionAndRotation(HoloKitCamera.Instance.transform.position +
                HoloKitCamera.Instance.transform.TransformVector(_cameraToImageOffset), HoloKitCamera.Instance.transform.rotation);
            go.transform.Rotate(go.transform.right, -90f);

            PosePosition = go.transform.position;
            HoriontalRotation = go.transform.rotation.eulerAngles.y;

            Destroy(go);
        }
    }

    public void StopSharingQRCode()
    {
        FindObjectOfType<QRCodeManager>().StopSharingQRCode();
    }
}
