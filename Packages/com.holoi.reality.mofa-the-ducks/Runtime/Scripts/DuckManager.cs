using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp.UI;
using HoloKit;

namespace Holoi.Reality.MOFATheDucks
{
    public class DuckManager : NetworkBehaviour
    {
        [SerializeField] private Duck _duckPrefab;

        /// <summary>
        /// The offset from the center eye to the spawn position of the newly spawned duck.
        /// </summary>
        private readonly Vector3 DuckSpawnOffset = new(0f, 0f, 0.5f);

        private void Awake()
        {
            HoloKitAppUIEventManager.OnStarUITriggered += OnStarUITriggered;
        }

        public override void OnDestroy()
        {
            HoloKitAppUIEventManager.OnStarUITriggered -= OnStarUITriggered;
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

        private void OnStarUITriggered()
        {
            Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
            Vector3 position = centerEyePose.position + centerEyePose.rotation * DuckSpawnOffset;
            Quaternion rotation = centerEyePose.rotation;

            SpawnDuck(position, rotation);
        }
    }
}
