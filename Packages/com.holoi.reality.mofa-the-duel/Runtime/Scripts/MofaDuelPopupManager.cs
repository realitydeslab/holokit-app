using UnityEngine;
using Unity.Netcode;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheDuel
{
    public class MofaDuelPopupManager : MofaPopupManager
    {
        [Header("MOFA The Duel")]
        [SerializeField] private GameObject _getReadyPrefab;

        [SerializeField] private GameObject _waitingOthersPrefab;

        protected override void Start()
        {
            base.Start();
            if (HoloKitApp.Instance.IsPlayer)
            {
                SpawnPopup(_getReadyPrefab);
                MofaPlayer.OnMofaPlayerReadyStateChanged += OnMofaPlayerReadyStateChanged;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (HoloKitApp.Instance.IsPlayer)
            {
                MofaPlayer.OnMofaPlayerReadyStateChanged -= OnMofaPlayerReadyStateChanged;
            }
        }

        private void OnMofaPlayerReadyStateChanged(ulong ownerClientId, bool ready)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                SpawnPopup(_waitingOthersPrefab);
            }
        }
    }
}
