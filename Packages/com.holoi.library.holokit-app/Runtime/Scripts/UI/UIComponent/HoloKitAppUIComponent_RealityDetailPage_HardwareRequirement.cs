using UnityEngine;
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
            _content.text = currentReality.HardwareRequirement;
        }
    }
}
