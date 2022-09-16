using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Mofa.Base
{
    public class ShieldSpell : NetworkBehaviour, IDamageable
    {
        public void OnHit(ulong clientId)
        {

        }
    }
}