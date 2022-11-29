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

        protected override void OnRoundData()
        {
            var summaryBoard = SpawnSummaryBoard();
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            // For the player, which in blue team
            var humanPlayer = mofaBaseRealityManager.Players[0];
            var humanPlayerStats = mofaBaseRealityManager.GetIndividualStats(humanPlayer);
            summaryBoard.BlueTeamName = "Blue";
            summaryBoard.BlueTeamKill = humanPlayerStats.Kill.ToString();
            summaryBoard.BlueTeamHitRate = humanPlayerStats.HitRate.ToString("F2");
            summaryBoard.BlueTeamDistance = humanPlayerStats.Distance.ToString("F2");

            // For the avatar, which is red team
            MofaPlayer secondPlayer = null;
            foreach (var player in mofaBaseRealityManager.Players.Values)
            {
                if (player.OwnerClientId == 0) continue;
                if (player.Team.Value == MofaTeam.Red)
                {
                    secondPlayer = player;
                    break;
                }
            }
            var aiPlayerStats = mofaBaseRealityManager.GetIndividualStats(secondPlayer);
            summaryBoard.RedTeamName = "Red";
            summaryBoard.RedTeamKill = aiPlayerStats.Kill.ToString();
            summaryBoard.RedTeamHitRate = aiPlayerStats.HitRate.ToString("F2");
            summaryBoard.RedTeamDistance = aiPlayerStats.Distance.ToString("F2");
        }
    }
}
