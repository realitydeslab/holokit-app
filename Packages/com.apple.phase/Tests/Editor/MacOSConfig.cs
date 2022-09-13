using UnityEditor;
using System;
using System.Linq;

#if UNITY_EDITOR_OSX
public class MacOSConfig
{
    [InitializeOnLoadMethod]
    public static void OnProjectLoad()
    {
        if (IsRunningTestsFromCommandline())
        {
            UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = true;
        }
    }

    internal static bool IsRunningTestsFromCommandline()
    {
        var commandLineArgs = Environment.GetCommandLineArgs();
        return commandLineArgs.Any(value => value == "-runTests");
    }
}
#endif