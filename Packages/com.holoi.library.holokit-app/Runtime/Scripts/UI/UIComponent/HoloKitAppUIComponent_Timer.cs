using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private void OnEnable()
        {
            UpdateTimer();
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            float sessionDuration = Time.time - HoloKitCamera.Instance.ARSessionStartTime;
            _timerText.text = HoloKitAppUtils.SecondToMMSS(sessionDuration);
        }
    }
}
