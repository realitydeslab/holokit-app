using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holoi.Library.HoloKitApp
{
    public static class HoloKitAppUtils
    {
        public static Quaternion GetHorizontalRotation(Quaternion rotation)
        {
            Vector3 cameraEuler = rotation.eulerAngles;
            return Quaternion.Euler(new Vector3(0f, cameraEuler.y, 0f));
        }

        public static double DegreeToRadian(double degree)
        {
            return Math.PI * degree / 180.0;
        }

        public static float PixelToMeter(int pixel)
        {
            return pixel / Screen.dpi / 39.3701f;
        }
    }
}
