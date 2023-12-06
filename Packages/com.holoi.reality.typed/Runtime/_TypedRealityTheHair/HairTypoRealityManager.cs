using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.Typography
{
    public class HairTypoRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        private void Start()
        {
            if (HoloKitApp.Instance.IsSpectator)
            {
                _arOcclusionManager.enabled = true;
            }
        }
    }
}