using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_ZeroVideoPadding : MonoBehaviour
    {
        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (currentReality.PreviewVideos.Count != 0 || currentReality.TutorialVideo != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
