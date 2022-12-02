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

                rootDict.SetString("NSCameraUsageDescription", "The app requires to use the camera to enable an immersive AR experience.");
                rootDict.SetString("NSMicrophoneUsageDescription", "The app can record audio through the microphone.");
                rootDict.SetString("NSPhotoLibraryAddUsageDescription", "The app saves recorded videos into Photo Library.");
                rootDict.SetString("NSPhotoLibraryUsageDescription", "The app saves recorded videos into Photo Library.");
                rootDict.SetString("NSLocationWhenInUseUsageDescription", "For location based services");
                //rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "For location based services");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}