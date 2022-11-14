using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkaDebugger : MonoBehaviour
{
    public void OnFireBallPointerDown()
    {
        Debug.Log("OnFireBallPointerDown");
    }

    public void OnFireBallPointerUp()
    {
        Debug.Log("OnFireBallPointerUp");
    }
}
