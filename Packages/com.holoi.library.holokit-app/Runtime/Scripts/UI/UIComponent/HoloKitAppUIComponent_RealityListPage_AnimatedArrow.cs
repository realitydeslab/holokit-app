using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityListPage_AnimatedArrow : MonoBehaviour
    {
        [SerializeField] private GameObject _arrow;

        private const float MovementDist = 30f;

        private const float MovementDuration = 1f;

        private void OnEnable()
        {
            _arrow.transform.localPosition = Vector3.zero;
            StartMoveForward();
        }

        private void OnDisable()
        {
            LeanTween.cancel(_arrow);
        }

        private void StartMoveForward()
        {
            LeanTween.moveLocalX(_arrow, -MovementDist, MovementDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(StartMoveBackward);
        }

        private void StartMoveBackward()
        {
            LeanTween.moveLocalX(_arrow, 0f, MovementDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(StartMoveForward);
        }
    }
}
