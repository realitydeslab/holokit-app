using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_ArrowChain_Animator : MonoBehaviour
    {
        [SerializeField] private HoloKitAppUIComponent_ArrowChain _arrowChain1;

        [SerializeField] private HoloKitAppUIComponent_ArrowChain _arrowChain2;

        private const float AnimationDuration = 99f;

        private void OnEnable()
        {
            _arrowChain1.transform.localPosition = new(-_arrowChain1.Length, 0f, 0f);
            _arrowChain2.transform.localPosition = Vector3.zero;
            StartArrowChainAnimation(_arrowChain1);
            StartArrowChainAnimation(_arrowChain2);
        }

        private void OnDisable()
        {
            LeanTween.cancel(_arrowChain1.gameObject);
            LeanTween.cancel(_arrowChain2.gameObject);
        }

        private void StartArrowChainAnimation(HoloKitAppUIComponent_ArrowChain arrowChain)
        {
            LeanTween.moveLocalX(arrowChain.gameObject, arrowChain.transform.localPosition.x + arrowChain.Length, AnimationDuration)
                .setEase(LeanTweenType.linear)
                .setOnComplete(() =>
                {
                    if (arrowChain.transform.localPosition.x == arrowChain.Length)
                    {
                        arrowChain.transform.localPosition = new(-arrowChain.Length, 0f, 0f);
                    }
                    StartArrowChainAnimation(arrowChain);
                });
        }
    }
}
