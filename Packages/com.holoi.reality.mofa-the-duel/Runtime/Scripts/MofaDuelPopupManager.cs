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

        protected override void UpdateSummaryBoard()
        {
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            if (mofaBaseRealityManager.CurrentPhase != MofaPhase.RoundData) return;

            var summaryBoard = SpawnSummaryBoard();
            // For the player, which in blue team
            var humanPlayer = mofaBaseRealityManager.PlayerDict[0];
            var humanPlayerStats = mofaBaseRealityManager.GetIndividualStats(humanPlayer);
            summaryBoard.BlueTeamName = "Blue";
            summaryBoard.BlueTeamKill = humanPlayerStats.Kill.ToString();
            summaryBoard.BlueTeamHitRate = humanPlayerStats.HitRate.ToString();
            summaryBoard.BlueTeamDistance = humanPlayerStats.Distance.ToString();
            summaryBoard.BlueTeamCalories = humanPlayerStats.Energy.ToString();

            // For the avatar, which is red team
            MofaPlayer secondPlayer = null;
            foreach (var player in mofaBaseRealityManager.PlayerDict.Values)
            {
                if (player.OwnerClientId == 0) continue;
                if (player.Team.Value == MofaTeam.Red)
                {
                    secondPlayer = player;
                    break;
                }
            }
            var secondPlayerStats = mofaBaseRealityManager.GetIndividualStats(secondPlayer);
            summaryBoard.RedTeamName = "Red";
            summaryBoard.RedTeamKill = secondPlayerStats.Kill.ToString();
            summaryBoard.RedTeamHitRate = secondPlayerStats.HitRate.ToString();
            summaryBoard.RedTeamDistance = secondPlayerStats.Distance.ToString();
            summaryBoard.RedTeamCalories = secondPlayerStats.Energy.ToString();
        }
    }
}
