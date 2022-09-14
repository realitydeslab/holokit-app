using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Holoi.HoloKit.App
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
