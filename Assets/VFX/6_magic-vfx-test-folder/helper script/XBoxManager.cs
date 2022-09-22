//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.InputSystem;
//using System.Collections;
//using System;

//public class XBoxManager : MonoBehaviour
//{
//    public GameObject targetObject;
//    public UnityEvent onTriggerEventA;
//    public UnityEvent onTriggerEventB;
//    public UnityEvent onTriggerEventX;
//    public UnityEvent onTriggerEventY;

//    void Update()
//    {
//        var gamepad = Gamepad.current;
//        if (gamepad == null)
//            return; // No gamepad connected.

//        if (gamepad.rightTrigger.wasPressedThisFrame)
//        {
//            // 'Use' code here
//        }

//        // transfrom by strick button
//        Vector2 move_L = gamepad.rightStick.ReadValue();
//        Vector2 move_R = gamepad.leftStick.ReadValue();

//        targetObject.transform.position += new Vector3(move_R.x , move_L.y, move_R.y) * 0.01f;



//        // event trigger
//        if (gamepad.aButton.wasPressedThisFrame)
//        {
//            onTriggerEventA?.Invoke();
//        }

//        if (gamepad.bButton.wasPressedThisFrame)
//        {
//            onTriggerEventB?.Invoke();
//        }

//        if(gamepad.xButton.wasPressedThisFrame)
//        {
//            onTriggerEventX?.Invoke();
//        }
//        if (gamepad.yButton.wasPressedThisFrame)
//        {
//            onTriggerEventY?.Invoke();
//        }

//        // dpad的右键（1），左键（-1）；上键（1），下键（-1）；构成一个vector2
//        if (gamepad.dpad.IsPressed())
//        {
//            var cc = gamepad.dpad.ReadValue();
//        }
//    }
//}