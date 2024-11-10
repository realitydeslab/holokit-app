// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_RecordButton : MonoBehaviour
    {
        public bool IsRecording => _image.sprite == _rectangle;

        [SerializeField] private Sprite _round;

        [SerializeField] private Sprite _rectangle;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        public void ToggleRecording()
        {
            var recorder = HoloKitApp.Instance.Recorder;
            if (IsRecording)
            {
                _image.sprite = _round;
                recorder.StopRecording();
            }
            else
            {
                _image.sprite = _rectangle;
                recorder.StartRecording();
            }
        }
    }
}
