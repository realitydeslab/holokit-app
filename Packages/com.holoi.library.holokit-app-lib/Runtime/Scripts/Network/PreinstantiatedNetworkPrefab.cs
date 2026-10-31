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

        /// <summary>
        /// Points the handler at a new instance. Shutting a client down hands every NetworkObject with a handler to
        /// Destroy, spawned or not, so a client whose connection attempt failed has lost its instance.
        /// </summary>
        internal static void Replace(NetworkManager manager, GameObject prefab, NetworkObject instance)
        {
            manager.PrefabHandler.RemoveHandler(prefab);
            if (!manager.PrefabHandler.AddHandler(prefab, new PreinstantiatedNetworkPrefab(instance)))
                throw new InvalidOperationException("Could not register the spawn handler again for " + prefab.name);
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
