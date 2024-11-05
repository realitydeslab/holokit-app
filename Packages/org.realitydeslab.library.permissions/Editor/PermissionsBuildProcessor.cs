// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-FileContributor: Botao Amber Hu <botao@reality.design>
// SPDX-License-Identifier: MIT

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace RealityDesignLab.Library.Permissions.Editor
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
                rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "For location based services");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}