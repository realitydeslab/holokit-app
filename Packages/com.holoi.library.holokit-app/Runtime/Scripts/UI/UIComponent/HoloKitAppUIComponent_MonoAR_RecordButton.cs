// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
