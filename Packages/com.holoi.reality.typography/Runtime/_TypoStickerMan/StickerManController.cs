using UnityEngine;
using UnityEngine.VFX;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class StickerManController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _vfx;

        private void Start()
        {
            ARObjectAdjuster.Instance.SetARObject(transform);
        }

        private void Update()
        {
            var hostHandPose = ((StickerManRealityManager)HoloKitApp.Instance.RealityManager).HostHandPose;

            if (hostHandPose.IsActive)
            {
                _vfx.SetBool("IsInteractable", true);
                _vfx.SetVector3("Hand", hostHandPose.transform.position);
            }
            else
            {
                //_vfx.SetBool("IsInteractable", false);
            }
        }
    }
}
