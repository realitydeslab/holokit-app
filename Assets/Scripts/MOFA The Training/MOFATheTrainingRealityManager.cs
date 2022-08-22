using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HoloKit;

public class MOFATheTrainingRealityManager : RealityManager
{
    [SerializeField] private NetworkedCube _networkedCubePrefab;

    public override void Spawned()
    {
        base.Spawned();

        if (App.Instance.IsMaster)
        {
            Runner.Spawn(_networkedCubePrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
