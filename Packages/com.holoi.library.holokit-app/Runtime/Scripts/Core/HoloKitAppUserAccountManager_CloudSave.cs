using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;

namespace Holoi.Library.HoloKitApp
{
    /// <summary>
    /// This part of the partial class is reponsible for CloudSave.
    /// </summary>
    public partial class HoloKitAppUserAccountManager
    {
        private const int RetryInterval = 10;

        /// <summary>
        /// Upload Apple user email and Apple user name to the cloud database.
        /// </summary>
        private async void CloudSave_UploadAppleUserData()
        {
            // Check if we have user data in the local storage
            if (!PlayerPrefs.HasKey(AppleUserEmailKey))
                return;

            bool userDataUploaded;
            // Check if user data has already been uploaded
            var keySet = new HashSet<string> { AppleUserEmailKey };
            try
            {
                Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(keySet);
                userDataUploaded = savedData.ContainsKey(AppleUserEmailKey);
            }
            catch (Exception)
            {
                // Wait and retry
                await Task.Delay(RetryInterval * 1000);
                CloudSave_UploadAppleUserData();
                return;
            }

            // Upload user data
            if (!userDataUploaded)
            {
                var data = new Dictionary<string, object>
                {
                    { AppleUserEmailKey, PlayerPrefs.GetString(AppleUserEmailKey) },
                    { AppleUserNameKey, PlayerPrefs.GetString(AppleUserNameKey, "Anonymous") }
                };
                try
                {
                    await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                }
                catch (Exception)
                {
                    // Wait and retry
                    await Task.Delay(RetryInterval * 1000);
                    CloudSave_UploadAppleUserData();
                    return;
                }
            }
        }
    }
}
