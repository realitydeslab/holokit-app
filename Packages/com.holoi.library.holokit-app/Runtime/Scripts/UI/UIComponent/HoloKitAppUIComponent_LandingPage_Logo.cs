using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_Logo : MonoBehaviour
    {
        [SerializeField] private Image _logoImage;

        private const float AppearTime = 0.6f;

        private void Start()
        {
            _logoImage.color = new Color(1f, 1f, 1f, 0f);
        }

        private void Update()
        {
            if (_logoImage.color.a < 1f)
            {
                _logoImage.color = new Color(1f, 1f, 1f, _logoImage.color.a + Time.deltaTime / AppearTime);
            }
        }
    }
}
