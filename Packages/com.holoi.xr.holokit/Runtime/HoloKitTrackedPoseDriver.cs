using UnityEngine;

namespace HoloKit
{
    public class HoloKitTrackedPoseDriver : MonoBehaviour
    {
        private void Awake()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame;
                HoloKitARSessionControllerAPI.RegisterARSessionUpdatedFrameDelegate();
            }
        }

        private void OnDestroy()
        {
            if (HoloKitHelper.IsRuntime)
            {
                HoloKitARSessionControllerAPI.UnregisterARSessionUpdatedFrameDelegate();
                HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame;
            }
        }

        private void OnARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            transform.SetPositionAndRotation(matrix.GetPosition(),
                                             matrix.rotation);
        }
    }
}
