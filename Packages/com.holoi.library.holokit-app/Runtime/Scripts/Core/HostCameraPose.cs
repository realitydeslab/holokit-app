using UnityEngine.Animations;
using Unity.Netcode;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class HostCameraPose : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            HoloKitApp.Instance.MultiplayerManager.SetNetworkHostCameraPose(this);
            if (IsServer)
            {
                var parentConstraint = gameObject.AddComponent<ParentConstraint>();
                ConstraintSource source = new();
                source.sourceTransform = HoloKitCamera.Instance.CenterEyePose;
                source.weight = 1f;
                parentConstraint.AddSource(source);
                parentConstraint.weight = 1f;
                parentConstraint.constraintActive = true;
            }
        }
    }
}
