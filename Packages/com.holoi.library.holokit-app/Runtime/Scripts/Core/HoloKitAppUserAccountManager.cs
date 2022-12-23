using UnityEngine;
using Unity.Services.Core;

namespace Holoi.Library.HoloKitApp
{
    [DisallowMultipleComponent]
    public partial class HoloKitAppUserAccountManager : MonoBehaviour
    {
        /// <summary>
        /// Set this to true to clear session token in the next run.
        /// This is only used for testing purporse.
        /// </summary>
        [SerializeField] private bool _clearSessionToken = false;

        private async void Awake()
        {
            // UGS can still be initialied when there is no network connection
            await UnityServices.InitializeAsync();
            Analytics_Init();
            SIWA_Init();
            Authentication_Init();
        }

        private void OnDestroy()
        {
            Analytics_Deinit();
        }

        private void Update()
        {
            SIWA_Update();
        }
    }
}
