using UnityEngine;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class ExtendedHandJoint : MonoBehaviour
    {
        [SerializeField] private HoloKitHandJoint _handJoint;

        [SerializeField] private float _extendedLength;

        private void Update()
        {
            Vector3 handJointPosition = HoloKitHandTracker.Instance.GetHandJointPosition(_handJoint);
            Vector3 direction = (handJointPosition - HoloKitCamera.Instance.CenterEyePose.position).normalized;
            transform.position = handJointPosition + _extendedLength * direction;
        }
    }
}