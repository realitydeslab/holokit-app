using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class RecordButton : MonoBehaviour
    {
        [SerializeField] Sprite _record;
        [SerializeField] Sprite _recording;

        public void StartRecording(bool active)
        {
            if (active)
            {
                transform.Find("Image").GetComponent<Image>().sprite = _recording;
            }
            else
            {
                transform.Find("Image").GetComponent<Image>().sprite = _record;
            }

        }
    }
}
