using UnityEngine;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonColliderController : MonoBehaviour, IDamageable
    {
        [SerializeField] private TheDragonController _theDragonController;

        [SerializeField] private int _multiplier = 1;

        public void OnDamaged(ulong attackerClientId)
        {
            _theDragonController.OnDamaged(_multiplier, attackerClientId);
        }
    }
}
