using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppUtils
    {
        public static Quaternion GetHorizontalRotation(Quaternion rotation)
        {
            Vector3 cameraEuler = rotation.eulerAngles;
            return Quaternion.Euler(new Vector3(0f, cameraEuler.y, 0f));
        }
    }
}
