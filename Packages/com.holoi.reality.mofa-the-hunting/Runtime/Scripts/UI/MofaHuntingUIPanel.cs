using UnityEngine;
using Holoi.Library.HoloKitApp;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFATheHunting.UI
{
    public class MofaHuntingUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MofaHunting";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] private Joystick _joystick;

        private void Start()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            if (!HoloKitApp.Instance.IsPuppeteer)
            {
                _joystick.gameObject.SetActive(false);
            }
        }
    }
}
