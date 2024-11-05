// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using UnityEngine;

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public class MofaFightingPanelScores : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text _blueTeamScore;

        [SerializeField] TMPro.TMP_Text _redTeamScore;

        [SerializeField] private GameObject _blueTeamMark;

        [SerializeField] private GameObject _redTeamMark;

        private MofaBaseRealityManager _mofaBaseRealityManager;

        // TODO: Refine this to fit more than 2 players
        private void Start()
        {
            _mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        }

        private void Update()
        {
            UpdateTeamMarks();
            UpdateScores();
        }

        private void UpdateTeamMarks()
        {
            var localMofaPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
            if (localMofaPlayer == null)
            {
                _blueTeamMark.SetActive(false);
                _redTeamMark.SetActive(false);
                return;
            }

            if (localMofaPlayer.Team.Value == MofaTeam.Blue)
            {
                _blueTeamMark.SetActive(true);
                _redTeamMark.SetActive(false);
                return;
            }

            if (localMofaPlayer.Team.Value == MofaTeam.Red)
            {
                _blueTeamMark.SetActive(false);
                _redTeamMark.SetActive(true);
                return;
            }
        }

        private void UpdateScores()
        {
            var mofaPlayerList = _mofaBaseRealityManager.MofaPlayerList;
            int blueTeamScore = mofaPlayerList.Where(t => t.Team.Value == MofaTeam.Blue).Sum(t => t.Kill.Value);
            int redTeamScore = mofaPlayerList.Where(t => t.Team.Value == MofaTeam.Red).Sum(t => t.Kill.Value);

            _blueTeamScore.text = blueTeamScore < 10 ? "0" + blueTeamScore : blueTeamScore.ToString();
            _redTeamScore.text = redTeamScore < 10 ? "0" + redTeamScore : redTeamScore.ToString();
        }
    }
}