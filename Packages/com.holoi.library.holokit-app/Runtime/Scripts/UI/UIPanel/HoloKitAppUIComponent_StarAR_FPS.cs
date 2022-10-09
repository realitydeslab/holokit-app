using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_StarAR_FPS : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fpsText;

        private const float UpdateInverval = 0.2f;

        private float _accumulatedTime = 0;

        private int _frameCount;

        private void OnEnable()
        {
            _fpsText.text = "FPS";
            _accumulatedTime = 0f;
            _frameCount = 0;
        }

        private void Update()
        {
            _accumulatedTime += Time.deltaTime;
            _frameCount++;
            if (_accumulatedTime > UpdateInverval)
            {
                int fps = Mathf.RoundToInt(_frameCount / _accumulatedTime);
                _fpsText.text = fps.ToString();

                _accumulatedTime = 0f;
                _frameCount = 0;
            }
        }
    }
}
