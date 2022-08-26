using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static string GetRandomSessionCode()
    {
        int length = 6;
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += characters[Random.Range(0, characters.Length - 1)];
        }
        return result;
    }

    public static string GetRandomMPCPassword()
    {
        int length = 8;
        string characters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string result = "";

        for (int i = 0; i < length; i++)
        {
            result += characters[Random.Range(0, characters.Length - 1)];
        }
        return result;
    }

    public static Vector3 GetHorizontalForward(Transform transform)
    {
        return new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
    }

    public static Quaternion GetHorizontalRotation(Quaternion rotation)
    {
        Vector3 cameraEuler = rotation.eulerAngles;
        return Quaternion.Euler(new Vector3(0f, cameraEuler.y, 0f));
    }

    public static float InverseClamp(float value, float min, float max)
    {
        if (value < max && value > min)
        {
            return 0f;
        }
        else
        {
            return value;
        }
    }

    public static float Remap(float input, float in_min, float in_max, float out_min, float out_max, bool isClamp)
    {
        if (isClamp)
        {
            if (input > in_max) input = in_max;
            if (input < in_min) input = in_min;
        }
        else
        {

        }
        float o = ((input - in_min) / (in_max - in_min)) * (out_max - out_min) + out_min;
        return o;
    }
}
