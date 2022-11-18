using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_RecordButton : MonoBehaviour
    {
        [HideInInspector] public bool IsRecording = false;

        [SerializeField] private Sprite _round;

        [SerializeField] private Sprite _rectangle;

        public void ToggleRecording()
        {
            if (IsRecording)
            {
                IsRecording = false;
                GetComponent<Image>().sprite = _round;
                HoloKitAppUIEventManager.OnStoppedRecording?.Invoke();
            }
            else
            {
                IsRecording = true;
                GetComponent<Image>().sprite = _rectangle;
                HoloKitAppUIEventManager.OnStartedRecording?.Invoke();
            }
        }
    }
}
