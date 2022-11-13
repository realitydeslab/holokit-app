using UnityEngine;
using UnityEngine.VFX;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : NetworkBehaviour
    {
        [SerializeField] private VisualEffect _fireBallVfx;

        public Transform DragonMousePose
        {
            get => _dragonMousePose;
            set
            {
                _dragonMousePose = value;
            }
        }

        private Transform _dragonMousePose;

        private bool _isCharging = true;

        // Meters per second
        private const float Speed = 5f;

        private void Update()
        {
            if (IsServer && _isCharging)
            {
                transform.SetPositionAndRotation(_dragonMousePose.position,
                                                 _dragonMousePose.rotation);
            }
        }

        // Host only
        public void FlyToTarget()
        {
            // Now the dragon will only attack the host.
            var targetPosition = ((MofaBaseRealityManager)HoloKitApp.Instance.RealityManager).Players[0].transform.position;
            float distance = Vector3.Distance(transform.position, targetPosition);
            _isCharging = false;
            LeanTween.move(gameObject, targetPosition, distance / Speed)
                .setOnComplete(() =>
                {
                    Destroy(gameObject, 1f);
                });
        }
    }
}
