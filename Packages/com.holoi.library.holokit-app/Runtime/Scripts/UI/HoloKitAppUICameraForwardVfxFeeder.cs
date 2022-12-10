using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUICameraForwardVfxFeeder : MonoBehaviour
    {
        private VisualEffect _vfx;

        private void Start()
        {
            _vfx = GetComponent<VisualEffect>();
        }

        private void Update()
        {
            _vfx.SetVector3("CameraForward", Camera.main.transform.forward);
        }
    }
}
