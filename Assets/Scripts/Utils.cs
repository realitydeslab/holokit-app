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
}
