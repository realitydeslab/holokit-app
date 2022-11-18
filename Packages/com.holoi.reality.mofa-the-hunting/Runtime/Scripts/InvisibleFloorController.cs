using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class InvisibleFloorController : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).SetInvisibleFloor(gameObject);
            //if (HoloKitUtils.IsRuntime)
            //{
            //    GetComponentInChildren<MeshRenderer>().enabled = false;
            //}
        }
    }
}
