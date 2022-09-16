using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanelScores : MonoBehaviour
    {
        private MofaBaseRealityManager _mofaRealityManager;

        private void OnEnable()
        {
            MofaPlayer.OnScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            MofaPlayer.OnScoreChanged -= OnScoreChanged;
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

            // SIZHENGTODO: 更新比分面板
            // blueTeamScore 为此时蓝队比分
            // redTeamScore 为此时红队比分
        }
    }
}