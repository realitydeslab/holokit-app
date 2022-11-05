using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_StarModeHelper : MonoBehaviour
    {
        [SerializeField] private GameObject _arrow;

        private const float _arrowInitialY = 100f;

        private const float _arrowMovingDistance = 30f;

        private const float _arrowMovingDuration = 0.8f;

        private void Start()
        {
            _arrow.GetComponent<RectTransform>().anchoredPosition = new(0f, _arrowInitialY);
            StartMovingUpward();
            StartCoroutine(HoloKitAppUtils.WaitAndDo(8f, () =>
            {
                gameObject.SetActive(false);
            }));
        }

        private void OnDisable()
        {
            LeanTween.cancel(_arrow);
            gameObject.SetActive(false);
        }

        private void StartMovingUpward()
        {
            LeanTween.moveLocalY(_arrow, _arrowInitialY + _arrowMovingDistance, _arrowMovingDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(StartMovingDownward);
        }

        private void StartMovingDownward()
        {
            LeanTween.moveLocalY(_arrow, _arrowInitialY, _arrowMovingDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(StartMovingUpward);
        }
    }
}
