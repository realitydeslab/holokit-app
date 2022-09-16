using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    public interface IDamageable
    {
        public void OnDamaged(ulong attackerClientId);
    }
}