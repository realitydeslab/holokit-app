using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp.UI;
using Holoi.Library.HoloKitApp;
using HoloKit;

namespace Holoi.Reality.MOFATheDucks
{
    public class DuckManager : NetworkBehaviour
    {
        [SerializeField] private Duck _duckPrefab;

        /// <summary>
        /// The position offset from the center eye to the spawn position of the newly spawned duck.
        /// </summary>
        [SerializeField] private Vector3 _duckSpawnPositionOffset = new(0f, 0f, 0.3f);

        [SerializeField] private Quaternion _duckSpawnRotationOffset = Quaternion.Euler(-45f, 0f, 0f);

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitAppUIEventManager.OnStarUITriggered += OnStarUITriggered_Host;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (HoloKitApp.Instance.IsHost)
            {
                HoloKitAppUIEventManager.OnStarUITriggered -= OnStarUITriggered_Host;
            }
        }

        /// <summary>
        /// Spawn the duck across the network.
        /// </summary>
        /// <param name="position">Initial position of the duck</param>
        /// <param name="rotation">Initial rotation of the duck</param>
        public void SpawnDuck(Vector3 position, Quaternion rotation)
        {
            var duckInstance = Instantiate(_duckPrefab, position, rotation);
            duckInstance.GetComponent<NetworkObject>().Spawn();
        }

        private void OnStarUITriggered_Host()
        {
            Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
            Vector3 position = centerEyePose.position + centerEyePose.rotation * _duckSpawnPositionOffset;
            Quaternion rotation = _duckSpawnRotationOffset * centerEyePose.rotation;

            SpawnDuck(position, rotation);
        }
    }
}
