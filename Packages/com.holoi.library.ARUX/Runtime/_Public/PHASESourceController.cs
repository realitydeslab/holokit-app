using UnityEngine;
using Apple.PHASE;

namespace Holoi.Library.ARUX
{
    public class PHASESourceController : MonoBehaviour
    {
        [SerializeField] private PHASESource _phaseSource;

        [SerializeField] private AudioSource _unityAudioSource;

        private void Awake()
        {
            if (HoloKitApp.HoloKitApp.Instance.GlobalSettings.PhaseEnabled
                && HoloKitApp.HoloKitApp.Instance.CurrentReality.IsPhaseRequired())
            {
                _phaseSource.enabled = true;
                _unityAudioSource.enabled = false;
            }
            else
            {
                _phaseSource.enabled = false;
                _unityAudioSource.enabled = true;
            }
        }

        private void OnDestroy()
        {
            if (_phaseSource.enabled && _phaseSource.IsPlaying())
            {
                _phaseSource.Stop();
            }
        }
    }
}
