using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Netcode.Transports.MultipeerConnectivity.Editor
{
    public static class MultipeerConnectivityTransportBuildProcessor
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

                // For MPC
                rootDict.SetString("NSLocalNetworkUsageDescription", "For connecting to nearby devices");
                PlistElementArray array = rootDict.CreateArray("NSBonjourServices");
                array.AddString("_holokit-0904._tcp");
                array.AddString("_holokit-0904._udp");

                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}