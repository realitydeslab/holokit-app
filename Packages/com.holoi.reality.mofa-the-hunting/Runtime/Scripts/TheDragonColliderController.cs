using UnityEngine;
using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonColliderController : MonoBehaviour, IDamageable
    {
        [SerializeField] private TheDragonController _theDragonController;

        [SerializeField] private int _multiplier = 1;

        private void Start()
        {
            if (!HoloKitApp.Instance.IsHost)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        public void OnDamaged(ulong attackerClientId)
        {
            _theDragonController.OnDamaged(_multiplier, attackerClientId);
        }
    }
}
