using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class ScreenEffectController : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;

        private void Start()
        {
            if (vfx == null)
            {
                vfx = GetComponent<VisualEffect>();
            }
            LifeShield.OnBeingHit += OnLifeShieldBeingHit;
        }

        private void OnDestroy()
        {
            LifeShield.OnBeingHit -= OnLifeShieldBeingHit;
        }

        public void OnLifeShieldBeingHit(ulong ownerClientId)
        {
            if (ownerClientId == NetworkManager.Singleton.LocalClientId)
            {
                vfx.SendEvent("OnBeingHit");
            }
        }
    }
}
