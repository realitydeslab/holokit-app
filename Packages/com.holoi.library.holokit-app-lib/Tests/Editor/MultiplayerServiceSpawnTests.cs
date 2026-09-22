using System.Collections;
using System.Reflection;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    public sealed class MultiplayerServiceSpawnTests
    {
        private NetworkManager _host;
        private NetworkManager _client;
        private GameObject _hostService;
        private GameObject _clientService;

        [UnityTest]
        public IEnumerator HostAndClientSpawnTheirExistingServiceWithoutDuplicates()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            yield return new EnterPlayMode();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.holoi.library.holokit-app-lib/Assets/Prefabs/Core/HoloKit App Multiplayer Manager.prefab");
            Assert.That(prefab, Is.Not.Null);
            // OS assigns a free loopback port to keep this isolated from other tests.
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = (ushort)((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            _host = CreateManager("Test Host", port);
            _hostService = Object.Instantiate(prefab);
            var hostObject = _hostService.GetComponent<NetworkObject>();
            PreinstantiatedNetworkPrefab.Register(_host, prefab, hostObject);
            Assert.That(_host.StartHost(), Is.True);
            Assert.That(hostObject.IsSpawned, Is.True, "Host startup must spawn the registered service.");

            _client = CreateManager("Test Client", port);
            _clientService = Object.Instantiate(prefab);
            var clientObject = _clientService.GetComponent<NetworkObject>();
            // Two peers share this test process; production has one singleton per process.
            typeof(NetworkObject).GetField("NetworkManagerOwner", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(clientObject, _client);
            PreinstantiatedNetworkPrefab.Register(_client, prefab, clientObject);
            Assert.That(_client.StartClient(), Is.True);
            var deadline = Time.realtimeSinceStartup + 10;
            while (!clientObject.IsSpawned && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(_client.IsConnectedClient, Is.True);
            Assert.That(clientObject.IsSpawned, Is.True, "Client must bind the server spawn to its existing service.");
            Assert.That(clientObject.NetworkObjectId, Is.EqualTo(hostObject.NetworkObjectId));
            Assert.That(Object.FindObjectsByType<HoloKitAppMultiplayerManager>(FindObjectsSortMode.None).Length,
                Is.EqualTo(2), "There must be exactly one service per peer.");

            hostObject.Despawn();
            deadline = Time.realtimeSinceStartup + 5;
            while (_clientService != null && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(_clientService == null, Is.True, "Remote despawn must release the pre-instantiated service.");
        }

        private static NetworkManager CreateManager(string name, ushort port)
        {
            var go = new GameObject(name);
            var transport = go.AddComponent<UnityTransport>();
            transport.SetConnectionData("127.0.0.1", port);
            var manager = go.AddComponent<NetworkManager>();
            manager.NetworkConfig = new NetworkConfig { NetworkTransport = transport, EnableSceneManagement = false };
            return manager;
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (_client != null) _client.Shutdown();
            if (_host != null) _host.Shutdown();
            yield return null;
            if (_client != null) Object.Destroy(_client.gameObject);
            if (_host != null) Object.Destroy(_host.gameObject);
            if (_clientService != null) Object.Destroy(_clientService);
            if (_hostService != null) Object.Destroy(_hostService);
            yield return null;
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}
