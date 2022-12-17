using UnityEngine;
using UnityEngine.Animations;

namespace Holoi.Library.ARUX
{
    public class HostCenterEyePoseController : MonoBehaviour
    {
        [SerializeField] private ParentConstraint _parentConstraint;

        private void Start()
        {
            if (HoloKitApp.HoloKitApp.Instance.IsHost)
            {
                _parentConstraint.constraintActive = true;
            }
            else
            {
                _parentConstraint.constraintActive = false;
            }
        }
    }
}
