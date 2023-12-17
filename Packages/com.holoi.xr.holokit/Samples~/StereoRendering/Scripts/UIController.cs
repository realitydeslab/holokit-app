using UnityEngine;
using TMPro;

namespace HoloKit.Samples.StereoscopicRendering
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _renderModeText;
        [SerializeField] private TMP_Text _recordButtonText;
        
        HoloKitVideoRecorder _recorder;

        private void Awake()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        public void ToggleRenderMode()
        {
            if (HoloKitCameraManager.Instance.RenderMode == HoloKitRenderMode.Stereo)
            {
                HoloKitCameraManager.Instance.RenderMode = HoloKitRenderMode.Mono;
            }
            else
            {
                //HoloKitCameraManager.Instance.RenderMode = HoloKitRenderMode.Stereo;

                // Skip NFC scanning
                HoloKitCameraManager.Instance.OpenStereoWithoutNFC("SomethingForNothing");
            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                _renderModeText.text = "Mono";
            }
            else
            {
                _renderModeText.text = "Stereo";
            }
        }

        public void OnPressRecordButton()
        {
            if (_recorder == null)
                _recorder = FindObjectOfType<HoloKitVideoRecorder>();
            if  (_recorder == null)
                return;
            
            _recorder.ToggleRecording();

            _recordButtonText.text = _recorder.IsRecording ? "Stop" : "Record";
            _recordButtonText.color = _recorder.IsRecording ? Color.red : Color.black;
        }
    }
}