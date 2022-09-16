using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Mofa.Base
{
    public class MofaFightingPanelScores : MonoBehaviour
    {
        private MofaBaseRealityManager _mofaRealityManager;

        private void Awake()
        {
            _mofaRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        private void OnEnable()
        {
            MofaPlayer.OnScoreChanged += OnScoreChanged;
            MofaBaseRealityManager.OnPhaseChanged += OnPhaseChanged;
        }

        private void OnDisable()
        {
            MofaPlayer.OnScoreChanged -= OnScoreChanged;
            MofaBaseRealityManager.OnPhaseChanged -= OnPhaseChanged;
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
            // blueTeamScore 为此时蓝队比分
            // redTeamScore 为此时红队比分
        }

        private void OnPhaseChanged(MofaPhase mofaPhase)
        {
            if (mofaPhase == MofaPhase.Fighting)
            {
                // SIZHENGTODO: 计时开始，80s一局
            }
        }
    }
}