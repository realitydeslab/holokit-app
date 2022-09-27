using System;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class LoadingUIPanel : MonoBehaviour
    {
        public event Action OnAnimationTrigger;

        public void AnimationTrigger()
        {
            OnAnimationTrigger?.Invoke();
        }

    }
}
