using UnityEngine;

namespace Holoi.Library.ARUX
{
    public static class MathHelpers
    {
        public static Vector3 Vector3Multipier(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static float Remap(float x, float inMin, float inMax, float outMin, float outMax, bool clamp = false)
        {
            if (clamp)
            {
                if (inMin <= inMax)
                {
                    if (x < inMin) return outMin;
                    if (x > inMax) return outMax;
                }
                else
                {
                    if (x < inMax) return outMin;
                    if (x > inMin) return outMax;
                }

            }

            x = (((x - inMin) / (inMax - inMin)) * (outMax - outMin)) + outMin;
            return x;
        }
    }
}
