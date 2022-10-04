using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR";

        [SerializeField] private HoloKitAppUIPanel_MonoAR_RecordButton _recordButton;

        public void OnSpectatorButtonPressed()
        {

        }

        public void OnStarButtonPressed()
        {

        }

        public void OnExitButtonPressed()
        {
            HoloKitAppUIEventManager.OnExit?.Invoke();
        }

        public void OnRecordButtonPressed()
        {
            _recordButton.ToggleRecording();
        }
    }
}
