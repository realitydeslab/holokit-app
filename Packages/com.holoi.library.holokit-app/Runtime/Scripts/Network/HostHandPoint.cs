using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public abstract class HostHandPoint : MonoBehaviour
    {
        [SerializeField] private NetworkHoloKitHandTracker _networkHandTracker;

        protected virtual void Start()
        {
            NetworkHoloKitHandTracker.OnHostHandValidityChanged += OnHostHandValidityChanged;
        }

        protected virtual void OnDestroy()
        {
            NetworkHoloKitHandTracker.OnHostHandValidityChanged -= OnHostHandValidityChanged;
        }

        protected abstract void OnHostHandValidityChanged(bool isValid);
    }
}
