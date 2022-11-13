using UnityEngine;
using Unity.Netcode;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : NetworkBehaviour
    {
        public Transform DragonMousePose
        {
            get => _dragonMousePose;
            set
            {
                _dragonMousePose = value;
            }
        }

        private Transform _dragonMousePose;

        private void Update()
        {
            if (IsServer)
            {
                transform.SetPositionAndRotation(_dragonMousePose.position,
                                                 _dragonMousePose.rotation);
            }
        }
    }
}
