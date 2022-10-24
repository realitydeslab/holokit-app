using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public interface IDamageable
    {
        public abstract bool OnHitDelegation { get; }

        public void OnDamaged(Transform bulletTransform, ulong attackerClientId);
    }
}