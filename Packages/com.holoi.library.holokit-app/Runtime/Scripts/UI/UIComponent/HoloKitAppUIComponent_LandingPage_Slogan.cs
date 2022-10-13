using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_Slogan : MonoBehaviour
    {
        [SerializeField] private Image _sloganImage;

        private bool _appeared;

        private const float AppearDelay = 2f;

        private const float AppearTime = 1f;

        private void Start()
        {
            _sloganImage.color = new Color(1f, 1f, 1f, 0f);
            StartCoroutine(HoloKitAppUtils.WaitAndDo(AppearDelay, () =>
            {
                _appeared = true;
            }));
        }

        private void Update()
        {
            if (_appeared)
            {
                if (_sloganImage.color.a < 1f)
                {
                    _sloganImage.color = new Color(1f, 1f, 1f, _sloganImage.color.a + Time.deltaTime / AppearTime);
                }
            }
        }
    }
}
