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

        [SerializeField] private TMP_Text _blueTeamCaloriesText;

        [Header("Red Team")]
        [SerializeField] private TMP_Text _redTeamNameText;

        [SerializeField] private TMP_Text _redTeamKillText;

        [SerializeField] private TMP_Text _redTeamHitRateText;

        [SerializeField] private TMP_Text _redTeamDistanceText;

        [SerializeField] private TMP_Text _redTeamCaloriesText;

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
                _blueTeamDistanceText.text = value + "m";
            }
        }

        public string BlueTeamCalories
        {
            set
            {
                _blueTeamCaloriesText.text = value + "kcal";
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

        public string RedTeamCalories
        {
            set
            {
                _redTeamCaloriesText.text = value + "kcal";
            }
        }
    }
}
