using UnityEngine;

namespace HoloKit
{
    public static class HoloKitUtils
    {
        public static bool IsEditor => Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool IsRuntime => Application.platform == RuntimePlatform.IPhonePlayer;

        public static Matrix4x4 RightHandedMatrixToLeftHandedMatrix(Matrix4x4 rightHandedMatrix)
        {
            Matrix4x4 leftHandedMatrix = new();
            leftHandedMatrix[0, 0] = rightHandedMatrix[0, 0];
            leftHandedMatrix[0, 1] = rightHandedMatrix[0, 2];
            leftHandedMatrix[0, 2] = rightHandedMatrix[0, 1];
            leftHandedMatrix[0, 3] = rightHandedMatrix[0, 3];

            leftHandedMatrix[1, 0] = rightHandedMatrix[2, 0];
            leftHandedMatrix[1, 1] = rightHandedMatrix[2, 2];
            leftHandedMatrix[1, 2] = rightHandedMatrix[2, 1];
            leftHandedMatrix[1, 3] = rightHandedMatrix[2, 3];

            leftHandedMatrix[2, 0] = rightHandedMatrix[1, 0];
            leftHandedMatrix[2, 1] = rightHandedMatrix[1, 2];
            leftHandedMatrix[2, 2] = rightHandedMatrix[1, 1];
            leftHandedMatrix[2, 3] = rightHandedMatrix[1, 3];

            leftHandedMatrix[3, 0] = rightHandedMatrix[3, 0];
            leftHandedMatrix[3, 1] = rightHandedMatrix[3, 2];
            leftHandedMatrix[3, 2] = rightHandedMatrix[3, 1];
            leftHandedMatrix[3, 3] = rightHandedMatrix[3, 3];
            return leftHandedMatrix;
        }
    }
}
