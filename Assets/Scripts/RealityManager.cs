using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HoloKit;

public abstract class RealityManager : NetworkBehaviour
{
    [SerializeField] private Vector3 _cameraToImageOffset;

    [SerializeField] private AlignmentMark _alignmentMarkPrefab;

    //[Networked]
    //public Vector3 PosePosition { get; set; }

    //[Networked]
    //public float HoriontalRotation { get; set; }

    private AlignmentMark _alignmentMark;

    public override void Spawned()
    {
        App.Instance.SetRealityManager(this);
    }

    public override void FixedUpdateNetwork()
    {
        //if (Object.HasStateAuthority && HoloKitCamera.Instance != null)
        //{
        //    GameObject go = new GameObject();
        //    go.transform.SetPositionAndRotation(HoloKitCamera.Instance.transform.position +
        //        HoloKitCamera.Instance.transform.TransformVector(_cameraToImageOffset), HoloKitCamera.Instance.transform.rotation);
        //    go.transform.Rotate(go.transform.right, -90f);

        //    PosePosition = go.transform.position;
        //    HoriontalRotation = go.transform.rotation.eulerAngles.y;

        //    Destroy(go);
        //}
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnMarkChecked()
    {
        Debug.Log("[RPC] OnMarkChecked");
        Runner.Despawn(_alignmentMark.Object);
    }

    private void SpawnAlignmentMark()
    {
        _alignmentMark = Runner.Spawn(_alignmentMarkPrefab, Vector3.zero, Quaternion.identity);
    }

    public void StartSharingQRCode()
    {
        SpawnAlignmentMark();
        FindObjectOfType<QRCodeManager>().StartSharingQRCode();
    }

    public void StopSharingQRCode()
    {
        FindObjectOfType<QRCodeManager>().StopSharingQRCode();
    }
}
