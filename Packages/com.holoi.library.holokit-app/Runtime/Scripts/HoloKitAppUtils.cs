using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppUtils
    {
        public static bool IsEditor => Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer;

        public static bool IsRuntime => Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
