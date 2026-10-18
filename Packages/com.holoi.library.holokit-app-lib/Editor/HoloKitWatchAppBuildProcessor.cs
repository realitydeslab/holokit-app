// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.HoloKitAppLib.Editor
{
    public class HoloKitWatchAppBuildProcessor
    {
        private const string WatchAppTargetName = "HoloKitWatchApp";

        private const string HealthShareUsageDescription = "Your workout data during the gameplay will be displayed after each round.";

        // Names the product being built, e.g. MOFA, rather than always HoloKit.
        private static string HealthUpdateUsageDescription =>
            $"Workouts tracked by {PlayerSettings.productName} on Apple Watch will be saved to HealthKit.";

        private static int FindBuildPhaseIndex(PBXProject project, string targetGuid, string buildPhaseName) 
        {
            var buildPhaseGuids = project.GetAllBuildPhasesForTarget(targetGuid);
            //find the index of the build phase
            for (int i = 0; i < buildPhaseGuids.Length; i++)
            {
                if (project.GetBuildPhaseName(buildPhaseGuids[i]) == buildPhaseName) 
                {
                    return i;
                }
            }
            return buildPhaseGuids.Length;
        }

        // https://github.com/Manurocker95/IronRuby-Test/blob/57f8b66e88d7df2e9bd7936e83777a79427f8e13/Assets/VirtualPhenix/Scripts/Editor/AppleWatch/VP_SetupWatchExtension.cs
        [PostProcessBuild]
        private static void AppleWatchSetup(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS) return;
            var settings = Resources.Load<HoloKitAppGlobalSettings>(HoloKitProductConfiguration.ResourceName);
            if (settings == null || settings.AppConfig == null)
                throw new UnityEditor.Build.BuildFailedException("Missing product settings for Apple Watch packaging.");
            if (!settings.AppConfig.WatchCompanionEnabled) return;
            if (!Directory.Exists("WatchApp"))
            {
                throw new UnityEditor.Build.BuildFailedException("Apple Watch is enabled for this product, but its WatchApp folder is missing.");
            }

            string packageName = UnityEngine.Application.identifier;

            PBXProject project = new();
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projectPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();
            string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

            string watchAppTargetGuid = project.TargetGuidByName(WatchAppTargetName);

            // Unity can append to an existing Xcode export. The target then already exists, but
            // its copied sources, icon and build settings must still be refreshed for this build.
            bool isNewTarget = watchAppTargetGuid == null;
            if (isNewTarget)
            {
                watchAppTargetGuid = project.AddTarget(WatchAppTargetName, ".app", "com.apple.product-type.application");
            }

            // Copy Files
            string sourcePathPrefix = $"WatchApp/{WatchAppTargetName}";
            string destinationPathPrefix = "WatchApp";
            FileUtil.DeleteFileOrDirectory(Path.Combine(buildPath, destinationPathPrefix));

            // Copy the entire directory into Xcode
            FileUtil.CopyFileOrDirectory(sourcePathPrefix, Path.Combine(buildPath, destinationPathPrefix));

            if (isNewTarget)
            {
                AddWatchTargetContents(project, mainTargetGuid, frameworkTargetGuid, watchAppTargetGuid, destinationPathPrefix);
            }
            AddPrivacyManifest(project, watchAppTargetGuid, destinationPathPrefix);
            SetWatchBuildSettings(project, watchAppTargetGuid, packageName, destinationPathPrefix);
            project.WriteToFile(projectPath);
        }

        // App Store Connect refuses a Watch app that reads UserDefaults or the boot time without a privacy manifest.
        // Added on every build, not only to a new target, so an appended export gets it too.
        private static void AddPrivacyManifest(PBXProject project, string watchAppTargetGuid, string destinationPathPrefix)
        {
            string path = $"{destinationPathPrefix}/PrivacyInfo.xcprivacy";
            if (project.FindFileGuidByProjectPath(path) != null) return;
            string fileGuid = project.AddFile(path, path);
            project.AddFileToBuildSection(watchAppTargetGuid, project.GetResourcesBuildPhaseByTarget(watchAppTargetGuid), fileGuid);
        }

        private static void AddWatchTargetContents(PBXProject project, string mainTargetGuid, string frameworkTargetGuid,
            string watchAppTargetGuid, string destinationPathPrefix)
        {
            project.AddTargetDependency(mainTargetGuid, watchAppTargetGuid); 
            project.AddFrameworkToProject(frameworkTargetGuid, "WatchConnectivity.framework", true);

            project.AddSourcesBuildPhase(watchAppTargetGuid);
            project.AddHeadersBuildPhase(watchAppTargetGuid);
            project.AddResourcesBuildPhase(watchAppTargetGuid);
            project.AddFrameworksBuildPhase(watchAppTargetGuid);

            // Prevent duplication
            if (project.GetCopyFilesBuildPhaseByTarget(mainTargetGuid, "Embed Watch Content", "$(CONTENTS_FOLDER_PATH)/Watch", "16") == null)
            {
                int insertionIndex = FindBuildPhaseIndex(project, mainTargetGuid, "Unity Process symbols for Unity-iPhone");
                string sectionGuid = project.InsertCopyFilesBuildPhase(insertionIndex, mainTargetGuid, "Embed Watch Content", "$(CONTENTS_FOLDER_PATH)/Watch", "16");
                project.AddFileToBuildSection(mainTargetGuid, sectionGuid, project.GetTargetProductFileRef(watchAppTargetGuid));
            }

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
                "Foundation/HoloKitWatchAppManager.swift",
                "Foundation/Views/RootView.swift",
                "Foundation/Views/PanelView.swift",
                "Foundation/Views/PanelListView.swift",

                // Mofa
                "MOFA/MofaWatchAppManager.swift",
                "MOFA/Views/MofaRootView.swift",
                "MOFA/Views/MofaReadyView.swift",
                "MOFA/Views/MofaHandednessView.swift",
                "MOFA/Views/MofaFightingView.swift",
                "MOFA/Views/MofaResultView.swift",
                "MOFA/Views/MofaActivityRingView.swift"
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
            project.SetTeamId(watchAppTargetGuid, PlayerSettings.iOS.appleDeveloperTeamID);
        }

        private static void SetWatchBuildSettings(PBXProject project, string watchAppTargetGuid, string packageName,
            string destinationPathPrefix)
        {
            //Set Swift Version
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_NAME", WatchAppTargetName);
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", $"{packageName}.watch-app");
            project.SetBuildProperty(watchAppTargetGuid, "SWIFT_VERSION", "5.0");
            project.SetBuildProperty(watchAppTargetGuid, "GENERATE_INFOPLIST_FILE", "YES");
            project.SetBuildProperty(watchAppTargetGuid, "CURRENT_PROJECT_VERSION", PlayerSettings.iOS.buildNumber);
            project.SetBuildProperty(watchAppTargetGuid, "MARKETING_VERSION", PlayerSettings.bundleVersion);
            project.SetBuildProperty(watchAppTargetGuid, "TARGETED_DEVICE_FAMILY", "4");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_FILE", $"{destinationPathPrefix}/HoloKitWatchApp-Info.plist");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_CFBundleDisplayName", PlayerSettings.productName);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_UISupportedInterfaceOrientations",
                "UIInterfaceOrientationPortrait UIInterfaceOrientationPortraitUpsideDown");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_WKCompanionAppBundleIdentifier", packageName);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_NSHealthShareUsageDescription", HealthShareUsageDescription);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_NSHealthUpdateUsageDescription", HealthUpdateUsageDescription);
            project.SetBuildProperty(watchAppTargetGuid, "CODE_SIGN_ENTITLEMENTS", $"{destinationPathPrefix}/HoloKitWatchApp.entitlements");
            project.SetBuildProperty(watchAppTargetGuid, "ASSETCATALOG_COMPILER_APPICON_NAME", "AppIcon");
            project.SetBuildProperty(watchAppTargetGuid, "WATCHOS_DEPLOYMENT_TARGET", "9.0");
            project.SetBuildProperty(watchAppTargetGuid, "ARCHS", "$(ARCHS_STANDARD)");
            project.SetBuildProperty(watchAppTargetGuid, "SDKROOT", "watchos");
            project.SetBuildProperty(watchAppTargetGuid, "SUPPORTED_PLATFORMS", "watchsimulator watchos");
            project.SetBuildProperty(watchAppTargetGuid, "SKIP_INSTALL", "YES");
            // SetTeamId also adds target attributes and throws if they exist, so it runs only for a
            // new target; the build setting is refreshed on every export.
            project.SetBuildProperty(watchAppTargetGuid, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);
        }
    }
}
