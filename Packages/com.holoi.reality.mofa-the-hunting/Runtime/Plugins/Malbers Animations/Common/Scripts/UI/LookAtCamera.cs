using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>Used to align the UI to the Camera Direction</summary>
    [AddComponentMenu("Malbers/UI/Look At Camera")]

    public class LookAtCamera : MonoBehaviour
    {
        public bool justY = true;
        public Vector3 Offset;
        Transform cam;

        private void Start()
        {
            cam = Camera.main.transform;
        }

        void Update()
        {
            var lookPos = cam.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookPos);

                transform.eulerAngles = (new Vector3(justY ? 0 : rotation.eulerAngles.x, rotation.eulerAngles.y, 0) + Offset);
            }
        }
    }
}