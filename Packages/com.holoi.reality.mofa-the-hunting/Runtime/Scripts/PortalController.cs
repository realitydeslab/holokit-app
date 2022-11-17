using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class PortalController : NetworkBehaviour
    {
        [SerializeField] private float _lifetime = 10;

        private void Start()
        {
            Destroy(gameObject, _lifetime);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetPortalController(this);
        }
    }
}
