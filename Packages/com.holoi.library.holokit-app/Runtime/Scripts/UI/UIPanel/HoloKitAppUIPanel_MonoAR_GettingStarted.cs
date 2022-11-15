using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_GettingStarted : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_GettingStarted";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private GameObject _checkedImage;

        private bool _checked = false;

        private void Start()
        {
            _checkedImage.SetActive(false);
        }

        public void OnBackButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }

        public void OnToggleChecked()
        {
            if (_checked)
            {
                _checkedImage.SetActive(false);
                _checked = false;
            }
            else
            {
                _checkedImage.SetActive(true);
                _checked = true;
            }
        }

        public void OnEnterStarModeButtonPressed()
        {
            if (_checked)
            {
                HoloKitApp.Instance.GlobalSettings.InstructionEnabled = false;
            }
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("StarAR", HoloKitAppUICanvasType.StAR);
            HoloKitAppUIEventManager.OnRenderModeChanged?.Invoke(HoloKit.HoloKitRenderMode.Stereo);
        }
    }
}
