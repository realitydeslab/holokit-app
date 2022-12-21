using System.Linq;
using UnityEngine;
using TMPro;

namespace Holoi.Library.MOFABase
{
    public class SummaryBoard : MonoBehaviour
    {
        [Header("Blue Team")]
        [SerializeField] private TMP_Text _blueTeamNameText;

        [SerializeField] private TMP_Text _blueTeamKillText;

        [SerializeField] private TMP_Text _blueTeamHitRateText;

        [SerializeField] private TMP_Text _blueTeamDistanceText;

        [SerializeField] private TMP_Text _blueTeamEnergyText;

        [Header("Red Team")]
        [SerializeField] private TMP_Text _redTeamNameText;

        [SerializeField] private TMP_Text _redTeamKillText;

        [SerializeField] private TMP_Text _redTeamHitRateText;

        [SerializeField] private TMP_Text _redTeamDistanceText;

        [SerializeField] private TMP_Text _redTeamEnergyText;

        public string BlueTeamName
        {
            set
            {
                _blueTeamNameText.text = value;
            }
        }

        public string BlueTeamKill
        {
            set
            {
                _blueTeamKillText.text = value;
            }
        }

        public string BlueTeamHitRate
        {
            set
            {
                _blueTeamHitRateText.text = value + "%";
            }
        }

        public string BlueTeamDistance
        {
            set
            {
                _blueTeamDistanceText.text = value + "ft";
            }
        }

        public string BlueTeamEnergy
        {
            set
            {
                _blueTeamEnergyText.text = value + "kcal";
            }
        }

        public string RedTeamName
        {
            set
            {
                _redTeamNameText.text = value;
            }
        }

        public string RedTeamKill
        {
            set
            {
                _redTeamKillText.text = value;
            }
        }

        public string RedTeamHitRate
        {
            set
            {
                _redTeamHitRateText.text = value + "%";
            }
        }

        public string RedTeamDistance
        {
            set
            {
                _redTeamDistanceText.text = value + "ft";
            }
        }

        public string RedTeamEnergy
        {
            set
            {
                _redTeamEnergyText.text = value + "kcal";
            }
        }

        // TODO: Upgrade this to support more than 2 players
        private void Update()
        {
            var mofaBaseRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
            var mofaPlayerList = mofaBaseRealityManager.MofaPlayerList;

            var blueTeamPlayer = mofaPlayerList.FirstOrDefault(t => t.Team.Value == MofaTeam.Blue);
            var blueTeamPlayerStats = mofaBaseRealityManager.GetPlayerStats(blueTeamPlayer);
            BlueTeamKill = blueTeamPlayerStats.Kill.ToString();
            BlueTeamHitRate = Mathf.RoundToInt(blueTeamPlayerStats.HitRate * 100).ToString();
            BlueTeamDistance = Mathf.RoundToInt(blueTeamPlayerStats.Distance * MofaUtils.MeterToFoot).ToString();
            BlueTeamEnergy = Mathf.RoundToInt(blueTeamPlayerStats.Energy).ToString();
    
            var redTeamPlayer = mofaPlayerList.FirstOrDefault(t => t.Team.Value == MofaTeam.Red);
            var redTeamPlayerStats = mofaBaseRealityManager.GetPlayerStats(redTeamPlayer);
            RedTeamKill = redTeamPlayerStats.Kill.ToString();
            RedTeamHitRate = Mathf.RoundToInt(redTeamPlayerStats.HitRate * 100).ToString();
            RedTeamDistance = Mathf.RoundToInt(redTeamPlayerStats.Distance * MofaUtils.MeterToFoot).ToString();
            RedTeamEnergy = Mathf.RoundToInt(redTeamPlayerStats.Energy).ToString();
        }
    }
}
