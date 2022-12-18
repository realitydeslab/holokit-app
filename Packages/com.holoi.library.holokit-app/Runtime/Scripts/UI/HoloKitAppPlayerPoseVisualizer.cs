using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppPlayerPoseVisualizer : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text _distanceText;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(HoloKit.HoloKitCamera.Instance.CenterEyePose);
        }

        public void UpdateDistance(float distance)
        {
            _distanceText.text = "" + distance;
        }
    }
}
