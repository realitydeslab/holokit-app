using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_RecordButton : HoloKitAppUITemplate_StarAR_HorizontalScrollButton
    {
        public override bool SwipeRight => false;

        [SerializeField] private HoloKitAppUIPanel_StarAR _starARPanel;

        [SerializeField] private TMP_Text _recordText;

        [SerializeField] private GameObject _dot;

        [SerializeField] private Image _dotImage;

        [SerializeField] private TMP_Text _triggeredText;

        private bool _isRecording;

        private float _startRecordingTime;

        private readonly Color32 _activeColor = new(255, 91, 26, 255);

        private readonly Color32 _inactiveColor = new(255, 255, 255, 255);

        protected override void Update()
        {
            base.Update();

            if (_isRecording && !Selected)
            {
                float recordingTime = Time.time - _startRecordingTime;
                _recordText.text = HoloKitAppUtils.SecondToMMSS(recordingTime);
            }
        }

        protected override void OnSelected()
        {
            base.OnSelected();
            _dot.SetActive(false);
            if (_isRecording)
            {
                _recordText.text = "STOP";
                _recordText.color = _inactiveColor;
            }
            _starARPanel.OnRecordButtonPressed();
        }

        protected override void OnRecovered()
        {
            base.OnRecovered();
            _dot.SetActive(true);
            if (_isRecording)
            {
                _recordText.text = HoloKitAppUtils.SecondToMMSS(0f);
                _recordText.color = _activeColor;
            }
            _starARPanel.OnRecordButtonReleased();
        }

        protected override void OnTriggerred()
        {
            base.OnTriggerred();
            if (_isRecording)
            {
                _isRecording = false;
                _recordText.text = "RECORD";
                _recordText.color = _inactiveColor;
                _dotImage.color = _inactiveColor;
                _triggeredText.text = "Recording Stopped";
                HoloKitAppUIEventManager.OnStoppedRecording?.Invoke();
            }
            else
            {
                _isRecording = true;
                _startRecordingTime = Time.time;
                _recordText.text = HoloKitAppUtils.SecondToMMSS(0f);
                _recordText.color = _activeColor;
                _dotImage.color = _activeColor;
                _triggeredText.text = "Recording Started";
                HoloKitAppUIEventManager.OnStartedRecording?.Invoke();
            }
        }
    }
}
