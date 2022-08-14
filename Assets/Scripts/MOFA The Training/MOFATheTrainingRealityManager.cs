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

        Debug.Log("MOFATheTrainingRealityManager spawned");

        if (App.Instance.IsMaster)
        {
            Runner.Spawn(_networkedCubePrefab, Vector3.zero, Quaternion.identity);
        }

        if (!App.Instance.IsMaster)
        {
            Debug.Log($"pose: {PosePosition} and {PoseRotation}");
            GameObject go = new GameObject();
            Transform trans = go.transform;
            trans.SetPositionAndRotation(PosePosition, PoseRotation);
            Matrix4x4 matrix = trans.localToWorldMatrix;
            Matrix4x4 inverseMatrix = matrix.inverse;
            Vector3 position = inverseMatrix.GetPosition();
            Quaternion rotation = inverseMatrix.rotation;
            Debug.Log($"inversed pose: {position} and {rotation}");
            HoloKitARSessionControllerAPI.ResetOrigin(position, rotation);
            Destroy(go);
        }
    }
}
