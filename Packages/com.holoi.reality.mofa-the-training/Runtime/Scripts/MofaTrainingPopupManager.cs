using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingPopupManager : MofaPopupManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private GameObject _findPlanePrefab;

        [SerializeField] private GameObject _placeAvatarPrefab;

        protected override void Start()
        {
            base.Start();

            if (HoloKitApp.Instance.IsHost)
            {
                SpawnPopup(_findPlanePrefab);
            }
        }

        public void OnFoundPlane()
        {
            SpawnPopup(_placeAvatarPrefab);
        }

        public void OnLostPlane()
        {
            SpawnPopup(_findPlanePrefab);
        }

        protected override void UpdateSummaryBoard()
        {
            var summaryBoard = SpawnSummaryBoard();
            var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            // For the player, which in blue team
            var humanPlayer = mofaBaseRealityManager.Players[0];
            var humanPlayerStats = mofaBaseRealityManager.GetIndividualStats(humanPlayer);
            summaryBoard.BlueTeamName = "Player";
            summaryBoard.BlueTeamKill = humanPlayerStats.Kill.ToString();
            summaryBoard.BlueTeamHitRate = humanPlayerStats.HitRate.ToString();
            summaryBoard.BlueTeamDistance = humanPlayerStats.Distance.ToString();
            summaryBoard.BlueTeamCalories = humanPlayerStats.Calories.ToString();

            // For the avatar, which is red team
            var aiPlayer = mofaBaseRealityManager.Players[MofaPlayerAI.AIClientId];
            var aiPlayerStats = mofaBaseRealityManager.GetIndividualStats(aiPlayer);
            summaryBoard.RedTeamName = "Avatar";
            summaryBoard.RedTeamKill = aiPlayerStats.Kill.ToString();
            summaryBoard.RedTeamHitRate = aiPlayerStats.HitRate.ToString();
            summaryBoard.RedTeamDistance = aiPlayerStats.Distance.ToString();
            summaryBoard.RedTeamCalories = aiPlayerStats.Calories.ToString();
        }
    }
}
