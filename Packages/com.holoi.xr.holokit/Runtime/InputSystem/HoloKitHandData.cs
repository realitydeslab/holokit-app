using UnityEngine;

namespace HoloKit
{
  public enum HandJoint_Unity
  {
    wrist = 0,
    thumb_metacarpal = 1,
    thumb_phalanx_proximal = 2,
    thumb_phalanx_distal = 3,
    thumb_tip = 4,
    index_finger_metacarpal = 5,
    index_finger_phalanx_proximal = 6,
    index_finger_phalanx_intermediate = 7,
    index_finger_phalanx_distal = 8,
    index_finger_tip = 9,
    middle_finger_metacarpal = 10,
    middle_finger_phalanx_proximal = 11,
    middle_finger_phalanx_intermediate = 12,
    middle_finger_phalanx_distal = 13,
    middle_finger_tip = 14,
    ring_finger_metacarpal = 15,
    ring_finger_phalanx_proximal = 16,
    ring_finger_phalanx_intermediate = 17,
    ring_finger_phalanx_distal = 18,
    ring_finger_tip = 19,
    pinky_finger_metacarpal = 20,
    pinky_finger_phalanx_proximal = 21,
    pinky_finger_phalanx_intermediate = 22,
    pinky_finger_phalanx_distal = 23,
    pinky_finger_tip = 24
  }

  [System.Serializable]
  public class HandData
    {
    public int frame;
    public bool enabled;
    public int hand;
    public float trigger;
    public float squeeze;
    public Vector3 pointerPosition;
    public Quaternion pointerRotation;
    public HandJointData[] joints = new HandJointData[25];
  }

  [System.Serializable]
  public struct HandJointData
  {
    public Vector3 position;
    public Quaternion rotation;
    public float radius;
  }
}
