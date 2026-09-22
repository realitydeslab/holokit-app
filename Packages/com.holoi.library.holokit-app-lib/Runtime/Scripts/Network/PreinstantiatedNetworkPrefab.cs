using System;
using Unity.Netcode;
using UnityEngine;

namespace Holoi.Library.HoloKitAppLib
{
    /// <summary>Preserves a scene-local service reference when NGO synchronizes its spawn.</summary>
    internal sealed class PreinstantiatedNetworkPrefab : INetworkPrefabInstanceHandler
    {
        private readonly NetworkObject _instance;

        private PreinstantiatedNetworkPrefab(NetworkObject instance) => _instance = instance;

        internal static void Register(NetworkManager manager, GameObject prefab, NetworkObject instance)
        {
            manager.AddNetworkPrefab(prefab);
            if (!manager.PrefabHandler.AddHandler(prefab, new PreinstantiatedNetworkPrefab(instance)))
                throw new InvalidOperationException("A spawn handler is already registered for " + prefab.name);
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            if (_instance == null || _instance.IsSpawned)
                throw new InvalidOperationException("The pre-instantiated network service is unavailable.");
            _instance.transform.SetPositionAndRotation(position, rotation);
            return _instance;
        }

        public void Destroy(NetworkObject networkObject)
        {
            if (networkObject != null) UnityEngine.Object.Destroy(networkObject.gameObject);
        }
    }
}
