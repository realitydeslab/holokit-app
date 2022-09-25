using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public interface IDamageable
    {
        public void OnDamaged(ulong attackerClientId);
    }
}