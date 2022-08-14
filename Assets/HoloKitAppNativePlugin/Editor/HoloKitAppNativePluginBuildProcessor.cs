using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HoloKitAppNativePlugin.Editor {
    public static class HoloKitAppNativePluginBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                // For build settings
                //var projPath = buildPath + "/Unity-Iphone.xcodeproj/project.pbxproj";
                //var proj = new PBXProject();
                //proj.ReadFromFile(projPath);

                //var unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
                //var mainTargetGuid = proj.GetUnityMainTargetGuid();

                //proj.WriteToFile(projPath);

                // For info.plist
                string plistPath = buildPath + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                PlistElementDict rootDict = plist.root;

                // For MPC
                rootDict.SetString("NSLocalNetworkUsageDescription", "For connecting to nearby devices");
                PlistElementArray array = rootDict.CreateArray("NSBonjourServices");
                array.AddString("_holokit-812._tcp");
                array.AddString("_holokit-812._udp");

                //rootDict.SetString("NSCameraUsageDescription", "The app needs to access camera");
                //rootDict.SetString("NSMicrophoneUsageDescription", "The app needs to access microphone");
                //rootDict.SetString("NSPhotoLibraryUsageDescription", "The app needs to access photo library");
                //rootDict.SetString("NSPhotoLibraryAddUsageDescription", "The app needs to add to photo library");
                //rootDict.SetString("NSLocationWhenInUseUsageDescription", "The app needs to access location when in use");
                //rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "The app needs to access location always and when in use");
                //rootDict.SetString("NSHealthUpdateUsageDescription", "The app needs to access health update");
                //rootDict.SetString("NSHealthShareUsageDescription", "The app needs to access health share");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}