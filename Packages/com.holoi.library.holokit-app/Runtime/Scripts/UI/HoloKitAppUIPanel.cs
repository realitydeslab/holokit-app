using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUIPanel : MonoBehaviour
    {
        public abstract string UIPanelName { get; }

        public abstract bool OverlayPreviousPanel { get; }
    }
}
