using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Mofa.Base
{
    public class ScreenEffectController : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;

        private void OnEnable()
        {
            LifeShield.OnTopDestroyed += OnBeingHit;
            LifeShield.OnRightDestroyed += OnBeingHit;
            LifeShield.OnLeftDestroyed += OnBeingHit;
            LifeShield.OnBotDestroyed += OnBeingHit;
        }

        private void OnDisable()
        {
            LifeShield.OnTopDestroyed -= OnBeingHit;
            LifeShield.OnRightDestroyed -= OnBeingHit;
            LifeShield.OnLeftDestroyed -= OnBeingHit;
            LifeShield.OnBotDestroyed -= OnBeingHit;
        }

        private void Start()
        {
            if(vfx == null) vfx = GetComponent<VisualEffect>();
        }

        public void OnBeingHit(ulong id)
        {
            vfx.SendEvent("OnBeingHit");
        }
    }
}
