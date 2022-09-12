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
         const string WatchAppName = "HoloKit";

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

            string watchAppTargetGuid = project.TargetGuidByName($"{WatchAppName} Watch App");

            if (watchAppTargetGuid != null)
            {
                Debug.Log(watchAppTargetGuid);
            } else
            {
                watchAppTargetGuid = project.AddTarget($"{WatchAppName} Watch App", ".app", "com.apple.product-type.application");
            }


            project.AddTargetDependency(mainTargetGuid, watchAppTargetGuid); 
            project.AddFrameworkToProject(frameworkTargetGuid, "WatchConnectivity.framework", true);

            project.AddSourcesBuildPhase(watchAppTargetGuid);
            project.AddHeadersBuildPhase(watchAppTargetGuid);
            project.AddResourcesBuildPhase(watchAppTargetGuid);
            project.AddFrameworksBuildPhase(watchAppTargetGuid);

            string sectionGuid = project.AddCopyFilesBuildPhase(mainTargetGuid, "Embed Watch Content", "$(CONTENTS_FOLDER_PATH)/Watch", "16");
            project.AddFileToBuildSection(mainTargetGuid, sectionGuid, project.GetTargetProductFileRef(watchAppTargetGuid));

            // Copy Files
            FileUtil.DeleteFileOrDirectory(Path.Combine(buildPath, "WatchApp"));

            FileUtil.CopyFileOrDirectory("WatchApp", Path.Combine(buildPath, "WatchApp"));

            var filesToBuild = new List<string>
            {
                "WatchApp/Assets.xcassets",
                "WatchApp/Preview Content/Preview Assets.xcassets",
                "WatchApp/WatchApp.swift",
                "WatchApp/ViewModel.swift",
                "WatchApp/Views/ActivityRingsView.swift",
                "WatchApp/Views/ControlsView.swift",
                "WatchApp/Views/FightingView.swift",
                "WatchApp/Views/SessionPagingView.swift",
                "WatchApp/Views/SummaryView.swift"
            };

            foreach (var path in filesToBuild)
            {
                var fileGuid = project.AddFile(path, path);
                project.AddFileToBuild(watchAppTargetGuid, fileGuid);
            } 

            // var filesToAdd = new List<string>
            // {
            //     "WatchApp/Info.plist",
            //     "WatchApp/WatchApp.entitlements",
            // };

            // foreach (var path in filesToAdd)
            // {
            //     var fileGuid = project.AddFile(path, path);
            // } 

            //Add Library
            project.AddFrameworkToProject(watchAppTargetGuid, "HealthKit.framework", true);

            //Set Swift Version
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_NAME", $"{WatchAppName} Watch App");
            project.SetBuildProperty(watchAppTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", $"{packageName}.watchkitapp");
            project.SetBuildProperty(watchAppTargetGuid, "SWIFT_VERSION", "5.0");
            project.SetBuildProperty(watchAppTargetGuid, "GENERATE_INFOPLIST_FILE", "YES");
            project.SetBuildProperty(watchAppTargetGuid, "CURRENT_PROJECT_VERSION", PlayerSettings.iOS.buildNumber);
            project.SetBuildProperty(watchAppTargetGuid, "MARKETING_VERSION", PlayerSettings.bundleVersion);
            project.SetBuildProperty(watchAppTargetGuid, "TARGETED_DEVICE_FAMILY", "4");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_FILE", "WatchApp/Info.plist");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_WKCompanionAppBundleIdentifier", packageName);
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_UISupportedInterfaceOrientations",
                "UIInterfaceOrientationPortrait UIInterfaceOrientationPortraitUpsideDown");
            project.SetBuildProperty(watchAppTargetGuid, "WATCHOS_DEPLOYMENT_TARGET", "9.0");
            project.SetBuildProperty(watchAppTargetGuid, "INFOPLIST_KEY_CFBundleDisplayName", WatchAppName);
            project.SetBuildProperty(watchAppTargetGuid, "ARCHS", "$(ARCHS_STANDARD)");
            project.SetBuildProperty(watchAppTargetGuid, "SDKROOT", "watchos");
            project.SetBuildProperty(watchAppTargetGuid, "SUPPORTED_PLATFORMS", "watchsimulator watchos");
            project.SetTeamId(watchAppTargetGuid, PlayerSettings.iOS.appleDeveloperTeamID);

            project.WriteToFile(projectPath);


            // Add Entitlements
            string entitlementFileName = "WatchApp.entitlements";
            string entitlementPath = Path.Combine(Path.Combine(buildPath, "WatchApp"), entitlementFileName);
            ProjectCapabilityManager projectCapabilityManager = new ProjectCapabilityManager(projectPath, entitlementPath, null, watchAppTargetGuid);
            projectCapabilityManager.AddHealthKit();
            projectCapabilityManager.AddBackgroundModes(BackgroundModesOptions.None);
            projectCapabilityManager.WriteToFile();
        }
    }
}
