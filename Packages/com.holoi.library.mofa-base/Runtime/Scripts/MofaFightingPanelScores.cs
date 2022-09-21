using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanelScores : MonoBehaviour
    {
        private MofaBaseRealityManager _mofaRealityManager;
        [Header("UI Elenments")]
        [SerializeField] TMPro.TMP_Text _yourScore;
        [SerializeField] TMPro.TMP_Text _enemyScore;

        private void OnEnable()
        {
            MofaPlayer.OnScoreChanged += OnScoreChanged;
            RealityManager.OnRealityManagerSpawned += OnRealityManagerSpawned;
        }

        private void OnDisable()
        {
            MofaPlayer.OnScoreChanged -= OnScoreChanged;
            RealityManager.OnRealityManagerSpawned -= OnRealityManagerSpawned;
        }

        private void OnRealityManagerSpawned(RealityManager realityManager)
        {
            _mofaRealityManager = realityManager as MofaBaseRealityManager;
        }

        private void OnScoreChanged()
        {
            int blueTeamScore = 0;
            int redTeamScore = 0;
            foreach (var mofaPlayer in _mofaRealityManager.Players.Values)
            {
                if (mofaPlayer.Team.Value == MofaTeam.Blue)
                {
                    blueTeamScore += mofaPlayer.KillCount.Value;
                }
                else if (mofaPlayer.Team.Value == MofaTeam.Red)
                {
                    redTeamScore += mofaPlayer.KillCount.Value;
                }
            }

            Debug.Log($"[MofaFightingPanel] Blue {blueTeamScore} : Red {redTeamScore}");

            // SIZHENGTODO: 更新比分面板
            if (blueTeamScore < 10)
            {
                _yourScore.text = "0" + blueTeamScore;
            }
            else
            {
                _yourScore.text = "" + blueTeamScore;
            }

            if (redTeamScore < 10)
            {
                _enemyScore.text = "0" + redTeamScore;
            }
            else
            {
                _enemyScore.text = "" + redTeamScore;
            }
        }
    }
}