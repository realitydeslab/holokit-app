using UnityEngine;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class TornadoController : MonoBehaviour
    {
        private void Start()
        {
            ((TheTornadoRealityManager)HoloKitApp.Instance.RealityManager).GroundPosY.OnValueChanged += OnGroundPosYValueChanged;
        }

        private void OnDestroy()
        {
            var realityManager = (TheTornadoRealityManager)HoloKitApp.Instance.RealityManager;
            if (realityManager != null && realityManager.GroundPosY != null)
            {
                realityManager.GroundPosY.OnValueChanged -= OnGroundPosYValueChanged;
            }
        }

        private void OnGroundPosYValueChanged(float oldValue, float newValue)
        {
            transform.position = new(transform.position.x, newValue, transform.position.z);
        }
    }
}
