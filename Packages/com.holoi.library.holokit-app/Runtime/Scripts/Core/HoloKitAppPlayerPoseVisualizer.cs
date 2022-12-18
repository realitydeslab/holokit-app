using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppPlayerPoseVisualizer : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _distanceText;

        private void Update()
        {
            transform.LookAt(HoloKit.HoloKitCamera.Instance.CenterEyePose);
        }

        public void SetDistance(float distance)
        {
            _distanceText.text = Mathf.RoundToInt(distance).ToString();
        }
    }
}