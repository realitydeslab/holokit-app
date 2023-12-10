// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

#if APPLE_SIGNIN_ENABLED && UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using AppleAuth.Editor;

namespace Holoi.Library.HoloKitApp.Editor
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
            manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
            manager.WriteToFile();
        }
    }
}
#endif
