using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class UnkaDragonCollisionController : MonoBehaviour, IDamageable
    {
        [SerializeField] private UnkaDragonController _dragonController;

        [SerializeField] private int _damageMultiplier;

        private void Start()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        public void OnDamaged(ulong attackerClientId)
        {
            _dragonController.OnDamaged(_damageMultiplier, attackerClientId);
        }
    }
}
