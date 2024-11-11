// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_RealityGalleryPage_AnimatedArrow : MonoBehaviour
    {
        [SerializeField] private GameObject _arrow;

        private const float MovementDist = 45f;

        private const float MovementDuration = 0.6f;

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
