using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StARUIPanel : MonoBehaviour
{
    public static event Action OnTriggerBtnPressed;

    public void TriggerBtn()
    {
        Debug.Log("OnTriggerBtnPressed");
        OnTriggerBtnPressed?.Invoke();
    }
}
