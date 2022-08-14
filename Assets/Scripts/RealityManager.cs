using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class RealityManager : NetworkBehaviour
{
    [Networked]
    public Vector3 PosePosition { get; set; }

    [Networked]
    public Quaternion PoseRotation { get; set; }

    public override void Spawned()
    {
        App.Instance.SetRealityManager(this);
    }

    public void SetPose(Pose pose)
    {
        PosePosition = pose.position;
        PoseRotation = pose.rotation;
    }
}
