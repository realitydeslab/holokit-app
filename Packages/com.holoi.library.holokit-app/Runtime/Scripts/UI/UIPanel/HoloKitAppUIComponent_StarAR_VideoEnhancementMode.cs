using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_VideoEnhancementMode : MonoBehaviour
    {
        [SerializeField] private TMP_Text _videoEnhancementModeText;

        private void OnEnable()
        {
            if (HoloKitCamera.Instance.VideoEnhancementMode != VideoEnhancementMode.None)
            {
                _videoEnhancementModeText.text = HoloKitCamera.Instance.VideoEnhancementMode.ToString();
            }
            else
            {
                _videoEnhancementModeText.text = "";
            }
        }
    }
}
