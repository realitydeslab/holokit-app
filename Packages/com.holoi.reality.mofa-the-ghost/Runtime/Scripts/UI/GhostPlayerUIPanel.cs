using System;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFATheGhost.UI
{
    public class GhostPlayerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MOFATheGhost_Ghost";

        public override bool OverlayPreviousPanel => false;

        public static event Action OnTriggered;

        private void Start()
        {
            transform.SetAsFirstSibling();
        }

        public void OnTriggeredFunc()
        {
            OnTriggered?.Invoke();
        }
    }
}
