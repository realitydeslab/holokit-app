#if UNITY_INPUT_SYSTEM_1_4_4_OR_NEWER && XR_HANDS_1_3_OR_NEWER
using UnityEngine.Scripting;
using UnityEngine.XR.Hands;

namespace HoloKit
{
  [Preserve]
  class HoloKitHandsSubsystem : XRHandSubsystem
  {
    HoloKitHandsProvider handsProvider => provider as HoloKitHandsProvider;

    internal void UpdateHandJoints(HandData handData)
    {
      handsProvider.UpdateHandJoints(handData);
    }

    internal void SetUpdateHandsAllowed(bool allowed)
    {
      handsProvider.updateHandsAllowed = allowed;
    }

    internal void SetIsTracked(Handedness handedness, bool isTracked)
    {
      handsProvider.SetIsTracked(handedness, isTracked);
    }
  }
}
#endif