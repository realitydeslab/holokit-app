using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations
{
   // [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/malbers-input")]
    [AddComponentMenu("Malbers/Input/Mouse Scroll")]
    public class MMouseScroll : MonoBehaviour
    {
        public UnityEvent OnScrollUp = new UnityEvent();
        public UnityEvent OnScrollDown = new UnityEvent();

        private float mousedelta = 0;

        private void Update()
        {
            var newDelta = Input.mouseScrollDelta.y;

            if (newDelta != mousedelta)
            {
                mousedelta = newDelta;

                if (mousedelta < 0) OnScrollDown.Invoke();
                else if (mousedelta > 0) OnScrollUp.Invoke();
            }
        }
    }
}