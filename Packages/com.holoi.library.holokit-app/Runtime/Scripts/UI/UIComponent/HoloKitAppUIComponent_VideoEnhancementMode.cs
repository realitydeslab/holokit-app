using UnityEngine;
using TMPro;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_VideoEnhancementMode : MonoBehaviour
    {
        [SerializeField] private TMP_Text _videoEnhancementModeText;

        private void OnEnable()
        {
            if (HoloKitCamera.Instance.VideoEnhancementMode == VideoEnhancementMode.HighResWithHDR)
            {
                _videoEnhancementModeText.text = "4KHDR";
            }
            else if (HoloKitCamera.Instance.VideoEnhancementMode == VideoEnhancementMode.HighRes)
            {
                _videoEnhancementModeText.text = "4K";
            }
            else
            {
                _videoEnhancementModeText.text = "2K";
            }
        }
    }
}