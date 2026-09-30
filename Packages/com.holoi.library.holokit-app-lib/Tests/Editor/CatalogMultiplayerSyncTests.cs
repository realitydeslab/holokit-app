using System.Collections;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// For every scene in the open product's catalog, a host, a player (if the reality supports multiplayer)
    /// and a spectator (if it supports spectator view) join the same session in three Editors; the host starts hover-to-start games with the Trigger button; all three
    /// must end up with the same set of network objects, and the spectator must not count as a player or see a Trigger button.
    /// Run with <c>Tools/run_multiplayer_test.sh &lt;Product&gt; CatalogMultiplayerSyncTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class CatalogMultiplayerSyncTests
    {
        private const float Settle = 5f;

        [UnityTest, Timeout(3600000)]
        public IEnumerator EveryCatalogSceneSynchronizesHostPlayerAndSpectator()
        {
            MultiplayerScenario.RequireConfigured();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(Run());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Run()
        {
            var role = MultiplayerScenario.Role;
            var isHost = role == MultiplayerScenario.Host;
            yield return EditorAppDriver.WaitUntil(() => HoloKitApp.Instance != null, "App bootstrap did not create HoloKitApp.");
            var app = HoloKitApp.Instance;
            var gallery = SceneManager.GetActiveScene().name;
            var realities = app.GlobalSettings.GetAllRealities().ToArray();
            Assert.That(realities, Is.Not.Empty, "The product catalog is empty.");

            foreach (var reality in realities)
            {
                // Players join only multiplayer realities and spectators only spectator-view ones.
                var scene = reality.Scene.SceneName;
                var withPlayer = reality.IsMultiplayerSupported();
                var withSpectator = reality.IsSpectatorViewSupported();
                var joins = isHost || (role == MultiplayerScenario.Player ? withPlayer : withSpectator);
                if (!joins)
                {
                    yield return MultiplayerScenario.Barrier($"{scene}-synced");
                    MultiplayerScenario.Publish($"{scene}-left");
                    yield return MultiplayerScenario.Barrier($"{scene}-left");
                    continue;
                }
                if (!isHost) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, $"{scene}-hosting");
                var type = role == MultiplayerScenario.Spectator ? HoloKitAppPlayerType.Spectator : HoloKitAppPlayerType.Player;
                yield return EditorAppDriver.EnterReality(scene, isHost, type);
                if (isHost) MultiplayerScenario.Publish($"{scene}-hosting");
                yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager != null && app.MultiplayerManager.LocalPlayer != null,
                    $"{scene}: the {role} did not join.");
                MultiplayerScenario.Publish($"{scene}-joined", NetworkManager.Singleton.LocalClientId.ToString());

                if (isHost)
                {
                    if (withPlayer) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, $"{scene}-joined");
                    if (withSpectator) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, $"{scene}-joined");
                    var peers = 1 + (withPlayer ? 1 : 0) + (withSpectator ? 1 : 0);
                    // Count connections, not player objects: some realities add players of their own (Training's AI).
                    yield return EditorAppDriver.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsIds.Count == peers,
                        $"{scene}: host has {NetworkManager.Singleton.ConnectedClientsIds.Count} connections, expected {peers}.");
                    if (withSpectator)
                    {
                        var spectatorId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Spectator, $"{scene}-joined"));
                        yield return EditorAppDriver.WaitUntil(
                            () => app.MultiplayerManager.PlayerDict.TryGetValue(spectatorId, out var p) && p.PlayerType.Value == HoloKitAppPlayerType.Spectator,
                            $"{scene}: the spectator is not registered as a spectator.", 10f);
                    }
                    // Start games that wait for a hand, as a player would with the Trigger button.
                    var start = Object.FindAnyObjectByType<HoverableStartButton>();
                    if (start != null && start.isActiveAndEnabled) HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
                    yield return new WaitForSecondsRealtime(Settle);
                    MultiplayerScenario.Publish($"{scene}-objects", NetworkObjects());
                }
                else
                {
                    yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, $"{scene}-objects");
                    var expected = MultiplayerScenario.Read(MultiplayerScenario.Host, $"{scene}-objects");
                    yield return EditorAppDriver.WaitUntil(() => NetworkObjects() == expected,
                        $"{scene}: the {role} has network objects [{NetworkObjects()}], the host [{expected}].", 15f);
                    // Spectators cannot act, so they must not be offered a Trigger button that does nothing.
                    if (role == MultiplayerScenario.Spectator)
                        Assert.That(Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                            .Any(rect => rect.name == "Trigger Button"), Is.False, $"{scene}: the spectator sees a Trigger button.");
                }

                // Clients leave first, then the host ends the session; all return to the gallery.
                yield return MultiplayerScenario.Barrier($"{scene}-synced");
                if (isHost)
                {
                    if (withPlayer) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, $"{scene}-left");
                    if (withSpectator) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, $"{scene}-left");
                }
                if (NetworkManager.Singleton != null) app.Shutdown();
                else app.ExitNoLiDARScene();
                yield return EditorAppDriver.WaitUntil(() => SceneManager.GetActiveScene().name == gallery,
                    $"{scene}: the {role} did not return to the gallery.");
                yield return MultiplayerScenario.Barrier($"{scene}-left");
                yield return new WaitForSecondsRealtime(1f);
            }
            yield return MultiplayerScenario.Barrier("finished");
        }

        /// <summary>Spawned network objects as "id:name" in id order, comparable across Editors.</summary>
        private static string NetworkObjects() =>
            string.Join(",", NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                .OrderBy(o => o.NetworkObjectId).Select(o => $"{o.NetworkObjectId}:{o.name.Replace("(Clone)", "")}"));
    }
}
