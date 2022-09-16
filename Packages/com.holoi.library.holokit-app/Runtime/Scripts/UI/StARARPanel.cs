using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holoi.Library.HoloKitApp
{
    public class StARARPanel : MonoBehaviour
    {
        public static event Action OnTriggered;

        public static event Action OnTriggered2;

        public void OnTriggerButtonPressed()
        {
            OnTriggered?.Invoke();
        }

        public void OnTrigger2ButtonPressed()
        {
            OnTriggered2?.Invoke();
        }
    }
}
