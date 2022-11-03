using System;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public static class HoloKitAppUIEventManager
    {
        #region Screen AR Panel
        public static Action<HoloKitRenderMode> OnRenderModeChanged;

        public static Action OnStartedAdvertising;

        public static Action OnStoppedAdvertising;

        public static Action OnExitReality;

        public static Action OnStartedRecording;

        public static Action OnStoppedRecording;

        public static Action OnAlignmentMarkChecked;

        public static Action OnRescanQRCode;
        #endregion

        #region Star AR Panel
        public static Action OnTriggered;

        public static Action OnBoosted;
        #endregion

        #region Others
        public static Action OnExitNoLiDARScene;
        #endregion
    }
}