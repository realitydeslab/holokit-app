using UnityEngine;
using UnityEngine.VFX;
using HoloKit;

namespace Holoi.Library.ARUX
{
    public class CameraForwardVfxFeeder : MonoBehaviour
    {
        private VisualEffect _vfx;

        private void Start()
        {
            _vfx = GetComponent<VisualEffect>();
        }

        private void Update()
        {
            _vfx.SetVector3("CameraForward", HoloKitCamera.Instance.CenterEyePose.forward);
        }
    }
}
