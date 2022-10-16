using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealitySlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _realityId;

        [SerializeField] private TMP_Text _realityName;

        [SerializeField] private Button _hostButton;

        [SerializeField] private Button _clientButton;

        public void Init(Reality reality, int index)
        {
            _realityId.text = "Reality #" + HoloKitAppUtils.IntToStringF3(index);
            _realityName.text = reality.DisplayName;
            _hostButton.onClick.AddListener(() =>
            {
                HoloKitApp.Instance.CurrentReality = reality;
                HoloKitApp.Instance.EnterRealityAsHost();
            });
            _clientButton.onClick.AddListener(() =>
            {
                HoloKitApp.Instance.CurrentReality = reality;
                HoloKitApp.Instance.JoinRealityAsSpectator();
            });
        }
    }
}
