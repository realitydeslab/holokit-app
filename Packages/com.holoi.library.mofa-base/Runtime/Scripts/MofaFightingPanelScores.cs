using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public class MofaFightingPanelScores : MonoBehaviour
    {
        [Header("UI Elenments")]
        [SerializeField] TMPro.TMP_Text _yourScore;

        [SerializeField] TMPro.TMP_Text _enemyScore;

        [SerializeField] private GameObject _blueTeamMark;

        [SerializeField] private GameObject _redTeamMark;

        private void OnEnable()
        {
            MofaPlayer.OnScoreChanged += OnScoreChanged;
        }

        private void Start()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsHost)
            {
                _blueTeamMark.SetActive(true);
                _redTeamMark.SetActive(false);
            }
            else if (HoloKitApp.HoloKitApp.Instance.IsPlayer)
            {
                _blueTeamMark.SetActive(false);
                _redTeamMark.SetActive(true);
            }
            else if (HoloKitApp.HoloKitApp.Instance.IsSpectator)
            {
                _blueTeamMark.SetActive(false);
                _redTeamMark.SetActive(false);
            }
        }

        private void OnDisable()
        {
            MofaPlayer.OnScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged()
        {
            int blueTeamScore = 0;
            int redTeamScore = 0;
            var mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            foreach (var mofaPlayer in mofaRealityManager.Players.Values)
            {
                if (mofaPlayer.Team.Value == MofaTeam.Blue)
                {
                    blueTeamScore += mofaPlayer.Kill.Value;
                }
                else if (mofaPlayer.Team.Value == MofaTeam.Red)
                {
                    redTeamScore += mofaPlayer.Kill.Value;
                }
            }

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