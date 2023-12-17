using System;
using UnityEngine;
#if UNITY_INPUT_SYSTEM_1_4_4_OR_NEWER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
#if XR_HANDS_1_3_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;
#endif
#endif

namespace HoloKit
{
#if UNITY_INPUT_SYSTEM_1_4_4_OR_NEWER
  using InputSystem = UnityEngine.InputSystem.InputSystem;
#endif
  public class HoloKitInputSystem : MonoBehaviour
  {
#if UNITY_INPUT_SYSTEM_1_4_4_OR_NEWER
    private static bool initialized = false;
    private static HoloKitHMD hmd = null;

#if XR_HANDS_1_3_OR_NEWER
    private static HoloKitHandsSubsystem HoloKitHandsSubsystem = null;
    private static XRHandProviderUtility.SubsystemUpdater subsystemUpdater;
#endif

    private void Awake()
    {
      if (initialized)
      {
        return;
      }
      initialized = true;
      InputSystem.RegisterLayout<HoloKitHMD>(
        matches: new InputDeviceMatcher()
        .WithInterface("HoloKitHMD"));

#if XR_HANDS_1_3_OR_NEWER
      var descriptors = new List<XRHandSubsystemDescriptor>();
      SubsystemManager.GetSubsystemDescriptors(descriptors);
      for (var i = 0; i < descriptors.Count; ++i)
      {
        var descriptor = descriptors[i];
        if (descriptor.id == HoloKitHandsProvider.id)
        {
          HoloKitHandsSubsystem = descriptor.Create() as HoloKitHandsSubsystem;
          break;
        }
      }
      subsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(HoloKitHandsSubsystem);
#endif
    }

    private void OnEnable()
    {
      unsafe
      {
        //InputSystem.onDeviceCommand += HandleOnDeviceCommand;
      }
      //HoloKitManager.OnXRChange += OnXRChange;
      //HoloKitManager.OnHeadsetUpdate += OnHeadsetUpdate;
#if XR_HANDS_1_3_OR_NEWER
      //HoloKitManager.OnHandUpdate += OnHandUpdate;
      HoloKitHandsSubsystem?.Start();
      subsystemUpdater?.Start();
#endif
    }

    private void OnDisable()
    {
      unsafe
      {
        //InputSystem.onDeviceCommand -= HandleOnDeviceCommand;
      }
      RemoveAllDevices();
      //HoloKitManager.OnXRChange -= OnXRChange;
      //HoloKitManager.OnControllerUpdate -= OnControllerUpdate;
#if XR_HANDS_1_3_OR_NEWER
      //HoloKitManager.OnHandUpdate += OnHandUpdate;
      HoloKitHandsSubsystem?.Stop();
      subsystemUpdater?.Stop();
#endif
    }

#if XR_HANDS_1_3_OR_NEWER
    private void OnDestroy()
    {
      HoloKitHandsSubsystem?.Destroy();
      subsystemUpdater?.Destroy();
      HoloKitHandsSubsystem = null;
      subsystemUpdater = null;
    }
#endif

//    private static void OnXRChange(
//      HoloKitState state,
//      int viewsCount, Rect leftRect, Rect rightRect)
//    {
//      if (state == HoloKitState.NORMAL)
//      {
//        RemoveAllDevices();
//      }
//    }

    private static void RemoveAllDevices()
    {
      RemoveDevice(hmd);
      hmd = null;
#if XR_HANDS_1_3_OR_NEWER
      HoloKitHandsSubsystem?.SetUpdateHandsAllowed(false);
      DisableHandLeft();
      DisableHandRight();
#endif
    }

    private static void OnHeadsetUpdate(
        Matrix4x4 leftProjectionMatrix,
        Matrix4x4 rightProjectionMatrix,
        Quaternion leftRotation,
        Quaternion rightRotation,
        Vector3 leftPosition,
        Vector3 rightPosition)
    {
      SetHoloKitHMD();
      Vector3 devicePosition = leftPosition;
      Quaternion deviceRotation = leftRotation;
      //if (HoloKitManager.Instance.ViewsCount == 2)
      //{
      //  devicePosition = (leftPosition + rightPosition) * 0.5f;
      //}
      hmd.OnHeadsetUpdate(
        devicePosition, deviceRotation,
        leftRotation, rightRotation,
        leftPosition, rightPosition);
    }

#if XR_HANDS_1_3_OR_NEWER
    private static void OnHandUpdate(HandData handData)
    {
      HoloKitHandsSubsystem?.SetIsTracked((Handedness)handData.hand, handData.enabled);
      if (handData.enabled)
      {
        HoloKitHandsSubsystem?.SetUpdateHandsAllowed(true);
        HoloKitHandsSubsystem?.UpdateHandJoints(handData);
        MetaAimFlags aimFlags = MetaAimFlags.Computed | MetaAimFlags.Valid;
        if (handData.trigger > MetaAimHand.pressThreshold)
        {
          aimFlags |= MetaAimFlags.IndexPinching;
        }
        if (handData.hand == 1)
        {
          MetaAimHand.left ??= MetaAimHand.CreateHand(InputDeviceCharacteristics.Left);
          MetaAimHand.left.UpdateHand(
            true,
            aimFlags,
            new Pose(handData.pointerPosition, handData.pointerRotation),
            handData.trigger,
            0,
            0,
            0);
        }
        else
        {
          MetaAimHand.right ??= MetaAimHand.CreateHand(InputDeviceCharacteristics.Right);
          MetaAimHand.right.UpdateHand(
            true,
            aimFlags,
            new Pose(handData.pointerPosition, handData.pointerRotation),
            handData.trigger,
            0,
            0,
            0);
        }
      }
      else
      {
        if (handData.hand == 1)
        {
          DisableHandLeft();
        }
        else
        {
          DisableHandRight();
        }
      }
    }


    private static void DisableHandLeft()
    {
    }

    private static void DisableHandRight()
    {
       
    }


        // Hack - triggers XRInputModalityManager OnDeviceChange.
    private static IEnumerator RemoveDeviceAfterFrame(UnityEngine.InputSystem.InputDevice device)
    {
      yield return null;
      InputSystem.RemoveDevice(device);
    }
#endif
          
    private static void SetHoloKitHMD()
    {
      if (hmd != null)
      {
        return;
      }
      hmd = (HoloKitHMD)InputSystem.AddDevice(
        new InputDeviceDescription
        {
          interfaceName = "HoloKitHMD",
          product = "HoloKitHMD"
        });
    }

    private static void RemoveDevice(TrackedDevice device)
    {
      if (device != null && device.added)
      {
        InputSystem.RemoveDevice(device);
      }
    }
#endif
  }
}