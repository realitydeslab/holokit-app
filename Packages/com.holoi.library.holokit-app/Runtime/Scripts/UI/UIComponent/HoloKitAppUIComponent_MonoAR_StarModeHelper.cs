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
                HoloKitAppUIEventManager.OnStartedRecording += OnStartedRecording;
            }
        }

        private void OnDisable()
        {
            LeanTween.cancel(_arrow);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (!HoloKitApp.Instance.IsSpectator)
            {
                HoloKitAppUIEventManager.OnStartedRecording -= OnStartedRecording;
            }
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

        private void OnStartedRecording()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
