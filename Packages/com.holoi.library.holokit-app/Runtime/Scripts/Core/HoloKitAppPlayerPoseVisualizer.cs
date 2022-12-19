using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppPlayerPoseVisualizer : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _distanceText;

        /// <summary>
        /// The offset from the player pose to the pose visualizer center.
        /// </summary>
        public Vector3 Offset = new(0f, 0.2f, 0f);

        /// <summary>
        /// The corresponding player of this pose visualizer. The pose visualizer should
        /// always follow the corresponding player.
        /// </summary>
        public HoloKitAppPlayer Player;

        private void LateUpdate()
        {
            if (Player != null)
            {
                transform.position = Player.transform.position + Offset;
            }
            // The pose visualizer should always look at the local camera.
            transform.LookAt(HoloKit.HoloKitCamera.Instance.CenterEyePose);
        }

        public void SetDistance(float distance)
        {
            _distanceText.text = Mathf.RoundToInt(distance).ToString();
        }
    }
}