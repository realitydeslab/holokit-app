using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityListPage_ArrowQueue : MonoBehaviour
    {
        [SerializeField] private RectTransform _arrowQueue1;

        [SerializeField] private RectTransform _arrowQueue2;

        private const float MovementDuration = 27f;

        private const float InitialX = -2545f;

        private void Start()
        {
            StartFirstHalfMovement(_arrowQueue1);
            StartSecondHalfMovement(_arrowQueue2);
        }

        private void StartFirstHalfMovement(RectTransform arrowQueue)
        {
            LeanTween.moveLocalX(arrowQueue.gameObject, InitialX + arrowQueue.sizeDelta.x, MovementDuration)
                .setOnComplete(() =>
                {
                    StartSecondHalfMovement(arrowQueue);
                });
        }

        private void StartSecondHalfMovement(RectTransform arrowQueue)
        {
            LeanTween.moveLocalX(arrowQueue.gameObject, InitialX + _arrowQueue2.sizeDelta.x * 2f, MovementDuration)
                .setOnComplete(() =>
                {
                    arrowQueue.anchoredPosition = new Vector2(InitialX, 0f);
                    StartFirstHalfMovement(arrowQueue);
                });
        }
    }
}