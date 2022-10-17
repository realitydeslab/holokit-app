using UnityEngine;

namespace HoloKit
{
    public class HoloKitTrackedPoseDriver : MonoBehaviour
    {
        private readonly Matrix4x4 RotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, 90f));

        private void Awake()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame += OnARSessionUpdatedFrame;
        }

        private void OnDestroy()
        {
            HoloKitARSessionControllerAPI.OnARSessionUpdatedFrame -= OnARSessionUpdatedFrame;
        }

        private void OnARSessionUpdatedFrame(double timestamp, Matrix4x4 matrix)
        {
            if (HoloKitCamera.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                matrix = RotationMatrix * matrix;
            }
            transform.SetPositionAndRotation(matrix.GetPosition(),
                                             matrix.rotation);
        }
    }
}
