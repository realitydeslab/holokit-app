// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#if APPLE_SIGNIN_ENABLED && UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using AppleAuth.Editor;

namespace Holoi.Library.HoloKitAppLib.Editor
{
    public static class SignInWithAppleBuildProcessor
    {
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

            string packageName = UnityEngine.Application.identifier;
            string name = packageName.Substring(packageName.LastIndexOf('.') + 1);
            string entitlementFileName = name + ".entitlements";

            var manager = new ProjectCapabilityManager(projectPath, entitlementFileName, null, project.GetUnityMainTargetGuid());
            ProjectCapabilityManagerExtension.AddSignInWithAppleWithCompatibility(manager);
            manager.WriteToFile();
        }
    }
}
#endif
