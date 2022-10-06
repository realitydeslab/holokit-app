using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;
using UnityEditor.Build;
using System.Reflection;

namespace Holoi.Library.HoloKitApp.Editor
{
    public class WatchAppBuildProcessor
    {
        private const string WatchAppDisplayName = "HoloKit";

        private const string WatchAppTargetName = "HoloKitWatchApp";

        private const string HealthUsageDescription = "Please provide your health information";

        // https://github.com/Manurocker95/IronRuby-Test/blob/57f8b66e88d7df2e9bd7936e83777a79427f8e13/Assets/VirtualPhenix/Scripts/Editor/AppleWatch/VP_SetupWatchExtension.cs
        [PostProcessBuild]
        private static void AppleWatchSetup(BuildTarget target, string buildPath)
        {
            string packageName = UnityEngine.Application.identifier;

            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projectPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();
            string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

            string watchAppTargetGuid = project.TargetGuidByName(WatchAppTargetName);

            if (watchAppTargetGuid != null)
            {
                Debug.Log(watchAppTargetGuid);
            } else
            {
                watchAppTargetGuid = project.AddTarget(WatchAppTargetName, ".app", "com.apple.product-type.application");
            }

            project.AddTargetDependency(mainTargetGuid, watchAppTargetGuid); 
            project.AddFrameworkToProject(frameworkTargetGuid, "WatchConnectivity.framework", true);

            project.AddSourcesBuildPhase(watchAppTargetGuid);
            project.AddHeadersBuildPhase(watchAppTargetGuid);
            project.AddResourcesBuildPhase(watchAppTargetGuid);
            project.AddFrameworksBuildPhase(watchAppTargetGuid);

            // TODO: Might be wrong
            string sectionGuid = project.AddCopyFilesBuildPhase(mainTargetGuid, "Embed Watch Content", "$(CONTENTS_FOLDER_PATH)/WatchApp", "16");
            project.AddFileToBuildSection(mainTargetGuid, sectionGuid, project.GetTargetProductFileRef(watchAppTargetGuid));

            // Copy Files
            string sourcePathPrefix = $"WatchApp/{WatchAppTargetName}";
            string destinationPathPrefix = "WatchApp";
            FileUtil.DeleteFileOrDirectory(Path.Combine(buildPath, destinationPathPrefix));

            // Copy the entire directory into Xcode
            FileUtil.CopyFileOrDirectory(sourcePathPrefix, Path.Combine(buildPath, destinationPathPrefix));

            // Add each file in Xcode one by one
            var filesToBuild = new List<string>
            {
                "Assets.xcassets",
                "Preview Content/Preview Assets.xcassets",

                // Fonts
                "Fonts/OBJECTSANS_BOLD.ttf",
                "Fonts/OBJECTSANS_BOLDSLANTED.ttf",
                "Fonts/OBJECTSANS_HEAVY.ttf",
                "Fonts/OBJECTSANS_HEAVYSLANTED.ttf",
                "Fonts/OBJECTSANS_REGULAR.ttf",
                "Fonts/OBJECTSANS_SLANTED.ttf",
                "Fonts/OBJECTSANS_THIN.ttf",
                "Fonts/OBJECTSANS_THINSLANTED.ttf",

                // Code
                "HoloKitWatchApp.swift",
                "Foundation/HoloKitAppWatchManager.swift",
                "Foundation/Views/HomeView.swift",
                "MOFA/MofaWatchManager.swift",
                "MOFA/Views/MofaFightingView.swift",
                "MOFA/Views/MofaHandednessView.swift",
                "MOFA/Views/MofaHomeView.swift",
                "MOFA/Views/MofaIntroView.swift",
                "MOFA/Views/MofaResultView.swift",
            };

            foreach (var path in filesToBuild)
            {
                var fileGuid = project.AddFile($"{destinationPathPrefix}/{path}", $"{destinationPathPrefix}/{path}");
                project.AddFileToBuild(watchAppTargetGuid, fileGuid);
            }

            var filesToAdd = new List<string>
             {
                "HoloKitWatchApp.entitlements",
                "HoloKitWatchApp-Info.plist"
             };

            foreach (var path in filesToAdd)
            {
                _ = project.AddFile($"{destinationPathPrefix}/{path}", $"{destinationPathPrefix}/{path}");
            }

            //Add Library
            project.AddFrameworkToProject(watchAppTargetGuid, "HealthKit.framework", true);

            //Set Swift Version
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_NAME", WatchAppTargetName);
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", $"{packageName}.watch-app");
            project.SetBuildProperty(watchAppTargetGuid, "SWIFT_VERSION", "5.0");
            project.SetBuildProperty(watchAppTargetGuid, "GENERATE_INFOPLIST_FILE", "YES");
            project.SetBuildProperty(watchAppTargetGuid, "CURRENT_PROJECT_VERSION", PlayerSettings.iOS.buildNumber);
            project.SetBuildProperty(watchAppTargetGuid, "MARKETING_VERSION", PlayerSettings.bundleVersion);
            project.SetBuildProperty(watchAppTargetGuid, "TARGETED_DEVICE_FAMILY", "4");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_FILE", $"{destinationPathPrefix}/HoloKitWatchApp-Info.plist");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_CFBundleDisplayName", WatchAppDisplayName);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_UISupportedInterfaceOrientations",
                "UIInterfaceOrientationPortrait UIInterfaceOrientationPortraitUpsideDown");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_WKCompanionAppBundleIdentifier", packageName);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_NSHealthUpdateUsageDescription", HealthUsageDescription);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_NSHealthShareUsageDescription", HealthUsageDescription);
            project.SetBuildProperty(watchAppTargetGuid, "CODE_SIGN_ENTITLEMENTS", $"{destinationPathPrefix}/HoloKitWatchApp.entitlements");
            project.SetBuildProperty(watchAppTargetGuid, "ASSETCATALOG_COMPILER_APPICON_NAME", "AppIcon");
            project.SetBuildProperty(watchAppTargetGuid, "WATCHOS_DEPLOYMENT_TARGET", "9.0");
            project.SetBuildProperty(watchAppTargetGuid, "ARCHS", "$(ARCHS_STANDARD)");
            project.SetBuildProperty(watchAppTargetGuid, "SDKROOT", "watchos");
            project.SetBuildProperty(watchAppTargetGuid, "SUPPORTED_PLATFORMS", "watchsimulator watchos");
            project.SetTeamId(watchAppTargetGuid, PlayerSettings.iOS.appleDeveloperTeamID);

            project.WriteToFile(projectPath);
        }
    }
}
