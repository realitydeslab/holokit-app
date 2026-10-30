using System.Linq;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;

namespace HoloKit.Tests
{
    /// <summary>
    /// The Network Manager every reality instantiates connects iPhones with the Network framework transport
    /// (org.realitydeslab.netcode.transport.network-framework). Multipeer Connectivity, its predecessor, connected
    /// two iOS 27 iPhones in 8 of 20 attempts on 2026-10-30; the Network framework transport connected 20 of 20.
    /// </summary>
    public sealed class NetworkManagerPrefabTests
    {
        private const string PrefabPath = "Packages/com.holoi.library.holokit-app-lib/Assets/Prefabs/Core/Network Manager.prefab";
        private const string TransportType = "Netcode.Transports.NetworkFramework.NetworkFrameworkTransport";

        private static GameObject Prefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.That(prefab, Is.Not.Null, PrefabPath);
            return prefab;
        }

        [Test]
        public void IPhonesConnectWithTheNetworkFrameworkTransport()
        {
            var prefab = Prefab();
            var transport = prefab.GetComponent<NetworkManager>().NetworkConfig.NetworkTransport;
            Assert.That(transport, Is.Not.Null, "NetworkConfig has no transport.");
            Assert.That(transport.GetType().FullName, Is.EqualTo(TransportType));
            var leftovers = prefab.GetComponents<MonoBehaviour>()
                .Where(component => component == null || component.GetType().Name == "MultipeerConnectivityTransport");
            Assert.That(leftovers, Is.Empty, "A Multipeer Connectivity transport (or a missing script) is still on the prefab.");
            // HoloKitApp swaps in UnityTransport in the Editor, where the multi-Editor tests run over 127.0.0.1.
            Assert.That(prefab.GetComponent<UnityTransport>(), Is.Not.Null, "The Editor's UnityTransport is missing.");
        }

        // The host advertises only while the QR sharing panel is open (HoloKitAppMultiplayerManager.StartAdvertising), and a
        // spectator browses from the waiting-for-connection panel; both then accept and join without a prompt.
        [Test]
        public void TheAppStartsAdvertisingAndBrowsingItself()
        {
            var transport = Prefab().GetComponent<NetworkManager>().NetworkConfig.NetworkTransport;
            using var serialized = new SerializedObject(transport);
            bool Flag(string name)
            {
                var property = serialized.FindProperty(name);
                Assert.That(property, Is.Not.Null, $"{transport.GetType().Name} has no {name}.");
                return property.boolValue;
            }
            Assert.That(Flag("AutoAdvertise"), Is.False, "AutoAdvertise");
            Assert.That(Flag("AutoBrowse"), Is.False, "AutoBrowse");
            Assert.That(Flag("AutoApproveConnectionRequest"), Is.True, "AutoApproveConnectionRequest");
            Assert.That(Flag("AutoConnectToHost"), Is.True, "AutoConnectToHost");
        }
    }
}
