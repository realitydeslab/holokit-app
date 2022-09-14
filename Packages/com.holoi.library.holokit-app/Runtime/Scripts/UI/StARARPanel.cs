using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holoi.Library.HoloKitApp
{
    public class StARARPanel : MonoBehaviour
    {
        public static event Action OnTriggered;

        public void OnTriggerButtonPressed()
        {
            OnTriggered?.Invoke();
        }
    }
}
