using UnityEngine;

namespace HoloKit
{
    public class HoloKitSystemSettings : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float _screenBrightness = 1f;

        private void Start()
        {
            Screen.brightness = _screenBrightness;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            UnityEngine.iOS.Device.hideHomeButton = true;
        }

        private void Update()
        {
            if (Screen.brightness < _screenBrightness)
            {
                Screen.brightness = _screenBrightness;
            }
        }
    }
}