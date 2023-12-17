#if UNITY_INPUT_SYSTEM_1_4_4_OR_NEWER
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace HoloKit
{
  public struct HoloKitHMDState : IInputStateTypeInfo
  {
    public static FourCC Format => new FourCC('H', 'L', 'K', 'H');

    public readonly FourCC format => Format;

    [InputControl(layout = "Integer")]
    public int trackingState;

    [InputControl(layout = "Button")]
    public bool isTracked;

    [InputControl(layout = "Vector3", noisy = true, dontReset = true)]
    public Vector3 devicePosition;

    [InputControl(layout = "Quaternion", noisy = true, dontReset = true)]
    public Quaternion deviceRotation;

    [InputControl(layout = "Vector3", noisy = true, dontReset = true)]
    public Vector3 leftEyePosition;

    [InputControl(layout = "Quaternion", noisy = true, dontReset = true)]
    public Quaternion leftEyeRotation;

    [InputControl(layout = "Vector3", noisy = true, dontReset = true)]
    public Vector3 rightEyePosition;

    [InputControl(layout = "Quaternion", noisy = true, dontReset = true)]
    public Quaternion rightEyeRotation;

    [InputControl(layout = "Vector3", noisy = true, dontReset = true)]
    public Vector3 centerEyePosition;

    [InputControl(layout = "Quaternion", noisy = true, dontReset = true)]
    public Quaternion centerEyeRotation;
  }

#if UNITY_EDITOR
  [UnityEditor.InitializeOnLoad]
#endif
  [InputControlLayout(stateType = typeof(HoloKitHMDState),
    displayName = "HoloKit Headset", hideInUI = true)]
  public class HoloKitHMD : XRHMD
  {
    static HoloKitHMD()
    {
      InputSystem.RegisterLayout<HoloKitHMD>();
    }

    public void OnHeadsetUpdate(
        Vector3 devicePosition,
        Quaternion deviceRotation,
        Quaternion leftRotation,
        Quaternion rightRotation,
        Vector3 leftPosition,
        Vector3 rightPosition)
    {
      var state = new HoloKitHMDState
      {
        trackingState = 3,
        isTracked = true,
        devicePosition = devicePosition,
        deviceRotation = deviceRotation,
        leftEyePosition = leftPosition,
        leftEyeRotation = leftRotation,
        rightEyePosition = rightPosition,
        rightEyeRotation = rightRotation,
        centerEyePosition = devicePosition,
        centerEyeRotation = deviceRotation
      };
      InputSystem.QueueStateEvent(this, state);
    }
  }
}
#endif