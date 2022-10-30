using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityListPage_AnimatedArrow : MonoBehaviour
    {
        [SerializeField] private GameObject _arrow;

        private const float MovementDist = 18f;

        private const float MovementDuration = 1f;

        private void Start()
        {
            StartMoveForward();
        }

        private void StartMoveForward()
        {
            LeanTween.moveLocalX(_arrow, -MovementDist, MovementDuration)
                .setOnComplete(StartMoveBackward);
        }

        private void StartMoveBackward()
        {
            LeanTween.moveLocalX(_arrow, 0f, MovementDuration)
                .setOnComplete(StartMoveForward);
        }
    }
}
