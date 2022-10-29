#if UNITY_IOS
using System;
using System.IO;
using System.Reflection;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace HoloKit.Editor
{
    /// <summary>Processes the project files after the build is performed.</summary>
    class HoloKitBuildProcessor
    {
        class PostProcessor : IPostprocessBuildWithReport
        {
            // NB: Needs to be > 0 to make sure we remove the shader since the
            //     Input System overwrites the preloaded assets array
            public int callbackOrder => 1;

            public void OnPostprocessBuild(BuildReport report)
            {
                PostprocessBuild(report);
            }

            void PostprocessBuild(BuildReport report)
            {
                //AddXcodePlist(report.summary.outputPath);
                //AddXcodeCapabilities(report.summary.outputPath);
                AddXcodeBuildSettings(report.summary.outputPath);
            }

            static void AddXcodePlist(string path) 
            {
                string plistPath = path + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                PlistElementDict rootDict = plist.root;

                // NFC Plist
                rootDict.SetString("NFCReaderUsageDescription", "For NFC authentication");
                PlistElementArray nfcArray = rootDict.CreateArray("com.apple.developer.nfc.readersession.iso7816.select-identifiers");
                nfcArray.AddString("D2760000850101");

                File.WriteAllText(plistPath, plist.WriteToString());
            }

            private static void AddXcodeBuildSettings(string pathToBuiltProject)
            {
                string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
                PBXProject proj = new();
                proj.ReadFromString(File.ReadAllText(projPath));

                string mainTargetGuid = proj.GetUnityMainTargetGuid();
                string unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();

                proj.SetBuildProperty(mainTargetGuid, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");
                proj.SetBuildProperty(unityFrameworkTargetGuid, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");
                proj.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
                proj.AddBuildProperty(unityFrameworkTargetGuid, "LIBRARY_SEARCH_PATHS", "$(SDKROOT)/usr/lib/swift");
                proj.SetBuildProperty(mainTargetGuid, "SUPPORTS_MAC_DESIGNED_FOR_IPHONE_IPAD", "NO");

                proj.WriteToFile(projPath);
            }

            private static void AddXcodeCapabilities(string buildPath)
            {
                string projectPath = PBXProject.GetPBXProjectPath(buildPath);
                PBXProject project = new();
                project.ReadFromFile(projectPath);
                string target = project.GetUnityMainTargetGuid();

                string packageName = UnityEngine.Application.identifier;
                string name = packageName.Substring(packageName.LastIndexOf('.') + 1);
                string entitlementFileName = name + ".entitlements";
                string entitlementPath = Path.Combine(buildPath, entitlementFileName);
                ProjectCapabilityManager projectCapabilityManager = new(projectPath, entitlementFileName, null, target);
                PlistDocument entitlementDocument = AddNFCEntitlement(projectCapabilityManager);
                entitlementDocument.WriteToFile(entitlementPath);

                var projectInfo = projectCapabilityManager.GetType().GetField("project", BindingFlags.NonPublic | BindingFlags.Instance);
                project = (PBXProject)projectInfo.GetValue(projectCapabilityManager);

                var constructor = typeof(PBXCapabilityType).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(bool), typeof(string), typeof(bool) }, null);
                PBXCapabilityType nfcCapability = (PBXCapabilityType)constructor.Invoke(new object[] { "com.apple.NearFieldCommunicationTagReading", true, "", false });
                project.AddCapability(target, nfcCapability, entitlementFileName);

                projectCapabilityManager.WriteToFile();
            }

            private static PlistDocument AddNFCEntitlement(ProjectCapabilityManager projectCapabilityManager)
            {
                MethodInfo getMethod = projectCapabilityManager.GetType().GetMethod("GetOrCreateEntitlementDoc", BindingFlags.NonPublic | BindingFlags.Instance);
                PlistDocument entitlementDoc = (PlistDocument)getMethod.Invoke(projectCapabilityManager, new object[] { });

                PlistElementDict dictionary = entitlementDoc.root;
                PlistElementArray array = dictionary.CreateArray("com.apple.developer.nfc.readersession.formats");
                array.values.Add(new PlistElementString("TAG"));

                return entitlementDoc;
            }
        }
    }
}
#endif