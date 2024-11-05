// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using RealityDesignLab.Library.HoloKitApp.IOSNative;

namespace RealityDesignLab.Library.HoloKitApp
{
    /// <summary>
	/// Check if there is a newer version on the App Store.
	/// </summary>
    public class HoloKitAppVersionChecker : MonoBehaviour
    {
        private const string Url = "https://itunes.apple.com/lookup?bundleId=com.holoi.holokit.holokit-app";

        private void Start()
        {
            StartCoroutine(RequestLatestVersion());
        }

        private IEnumerator RequestLatestVersion()
        {
            UnityWebRequest request = UnityWebRequest.Get(Url);
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JSONNode rootNode = JSON.Parse(request.downloadHandler.text);
                JSONNode resultsNode = rootNode["results"];
                JSONNode versionNode = resultsNode[0]["version"];
                string latestVersion = versionNode.ToString().Replace("\"", "");
                if (!CheckVersion(latestVersion))
                {
                    HoloKitAppIOSNativeAPI.ShowUpdateAlert();
                }   
            }
            HoloKitAppIOSNativeAPI.ShowUpdateAlert();
            Destroy(gameObject);
        }

        private bool CheckVersion(string version)
        {
            string[] latestVersion = version.Split(".");
            string[] currentVersion = Application.version.Split(".");

            int currentVersionMajor = int.Parse(currentVersion[0]);
            int currentVersionMinor = int.Parse(currentVersion[1]);
            int currentVersionPatch = int.Parse(currentVersion[2]);
            int latestVersionMajor = int.Parse(latestVersion[0]);
            int latestVersionMinor = int.Parse(latestVersion[1]);
            int latestVersionPatch = int.Parse(latestVersion[2]);

            if (currentVersionMajor > latestVersionMajor)
                return true;
            if (currentVersionMajor < latestVersionMajor)
                return false;
            // currentVersionMajor == latestVersionMajor
            if (currentVersionMinor > latestVersionMinor)
                return true;
            if (currentVersionMinor < latestVersionMinor)
                return false;
            // currentVersionMinor == latestVersionMinor
            if (currentVersionPatch > latestVersionPatch)
                return true;
            if (currentVersionPatch < latestVersionPatch)
                return false;
            // currentVersionPatch == latestVersionPatch
            return true;
        }
    }
}
