using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp.UI;

namespace Holoi.Reality.MOFATheGhost.UI
{
    public class GhostPlayerUIPanel : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MOFATheGhost_Ghost";

        public override bool OverlayPreviousPanel => false;
    }
}
