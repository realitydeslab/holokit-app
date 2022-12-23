using UnityEngine;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIEventReactor : MonoBehaviour
    {
        private void OnRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                HoloKitCamera.Instance.OpenStereoWithoutNFC("SomethingForNothing");
            }
            else
            {
                HoloKitCamera.Instance.RenderMode = HoloKitRenderMode.Mono;
            }
        }

        private void OnExitReality()
        {
            HoloKitApp.Instance.Shutdown();
        }

        private void OnExitNoLiDARScene()
        {
            HoloKitApp.Instance.ExitNoLiDARScene();
        }
    }
}
