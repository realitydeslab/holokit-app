using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_MonoAR_StarModeHelper : MonoBehaviour
    {
        [SerializeField] private GameObject _arrow;

        private const float ArrowInitialY = 100f;

        private const float ArrowMovingDistance = 30f;

        private const float ArrowMovingDuration = 0.8f;

        private const float Duration = 8f;

        private void Start()
        {
            if (HoloKitApp.Instance.IsSpectator)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _arrow.GetComponent<RectTransform>().anchoredPosition = new(0f, ArrowInitialY);
                StartMovingUpward();
                StartCoroutine(HoloKitAppUtils.WaitAndDo(Duration, () =>
                {
                    gameObject.SetActive(false);
                }));
            }
        }

        private void OnDisable()
        {
            LeanTween.cancel(_arrow);
        }

        private void StartMovingUpward()
        {
            LeanTween.moveLocalY(_arrow, ArrowInitialY + ArrowMovingDistance, ArrowMovingDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(StartMovingDownward);
        }

        private void StartMovingDownward()
        {
            LeanTween.moveLocalY(_arrow, ArrowInitialY, ArrowMovingDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(StartMovingUpward);
        }
    }
}
