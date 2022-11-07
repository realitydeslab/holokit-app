using UnityEngine;
using Apple.PHASE;

namespace Holoi.Library.ARUX
{
    public class PHASEListenerController : MonoBehaviour
    {
        [SerializeField] private PHASEListener _phaseListener;

        [SerializeField] private AudioListener _unityListener;

        private void Start()
        {
            if (HoloKitApp.HoloKitApp.Instance.GlobalSettings.PhaseEnabled
                && HoloKitApp.HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
            {
                _phaseListener.enabled = true;
                _unityListener.enabled = false;
            }
            else
            {
                _phaseListener.enabled = false;
                _unityListener.enabled = true;
            }
        }
    }
}
