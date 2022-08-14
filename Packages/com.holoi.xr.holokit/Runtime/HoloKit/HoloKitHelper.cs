using UnityEngine;

namespace HoloKit
{
    public static class HoloKitHelper
    {
        public static bool IsEditor => Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool IsRuntime => Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
