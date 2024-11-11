// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitAppLib
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

        public static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;

            if (values.Count() > 0)
            {
                // Compute the Average
                double avg = values.Average();

                // Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together
                ret = Math.Sqrt(sum / values.Count());
            }
            return ret;
        }

        public static string SecondToMMSS(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            return timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }

        public static IEnumerator WaitAndDo(float t, Action toDo)
        {
            yield return new WaitForSeconds(t);
            toDo();
        }

        public static string IntToStringF3(int num)
        {
            if (num < 10)
            {
                return "00" + num.ToString();
            }
            else if (num < 100)
            {
                return "0" + num.ToString();
            }
            else
            {
                return num.ToString();
            }
        }
    }
}
