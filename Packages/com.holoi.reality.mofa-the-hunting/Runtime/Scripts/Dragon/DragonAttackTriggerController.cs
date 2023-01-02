using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class DragonAttackTriggerController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (HoloKitApp.Instance.IsHost)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (other.GetComponentInParent<NetworkObject>().OwnerClientId != 0)
                    {
                        damageable.OnDamaged(0);
                    }
                }
            }
        }
    }
}
