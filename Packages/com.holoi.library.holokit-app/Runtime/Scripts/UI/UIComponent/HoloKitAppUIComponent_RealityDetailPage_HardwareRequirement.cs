using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityDetailPage_HardwareRequirement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _content;

        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            if (currentReality.HardwareRequirement.Equals(""))
            {
                Destroy(gameObject);
                return;
            }

            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "en":
                    _content.text = currentReality.HardwareRequirement;
                    break;
                case "zh-Hans":
                    _content.text = currentReality.HardwareRequirement_Chinese;
                    break;
            }
        }
    }
}
