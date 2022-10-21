using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Holoi.Library.Permissions.Editor
{
    public static class PermissionsBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                // For info.plist
                string plistPath = buildPath + "/Info.plist";
                PlistDocument plist = new();
                plist.ReadFromFile(plistPath);
                PlistElementDict rootDict = plist.root;

                rootDict.SetString("NSCameraUsageDescription", "For AR");
                rootDict.SetString("NSMicrophoneUsageDescription", "For recording");
                rootDict.SetString("NSPhotoLibraryAddUsageDescription", "For saving recorded videos");
                rootDict.SetString("NSPhotoLibraryUsageDescription", "For saving recorded videos");
                //rootDict.SetString("NSLocationWhenInUseUsageDescription", "For location based services");
                //rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "For location based services");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}