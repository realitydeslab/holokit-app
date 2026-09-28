using System.Collections;
using System.IO;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.QuantumRealm;
using RealityDesignLab.QuantumRealm.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// Quantum Realm with three Editors: the host reveals Buddhas with a synthetic hand; a second
    /// client and a spectator follow. Run with
    /// <c>Tools/run_multiplayer_test.sh QRealm QuantumRealmMultiplayerTests</c>. The spec is in the README.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class QuantumRealmMultiplayerTests
    {
        private const string Scene = "QuantumRealm";
        private const int Dwells = 3;

        [UnityTest, Timeout(600000)]
        public IEnumerator ClientsFollowTheHostsBuddhas()
        {
            MultiplayerScenario.RequireConfigured();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(MultiplayerScenario.Role == MultiplayerScenario.Host ? HostRole() : ViewerRole());
            yield return new ExitPlayMode();
        }

        // Coroutines below run after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator HostRole()
        {
            yield return EditorAppDriver.EnterReality(Scene, true, HoloKitAppPlayerType.Player);
            MultiplayerScenario.Publish("hosting");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "joined");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, "joined");
            yield return EditorAppDriver.WaitUntil(() => HoloKitApp.Instance.MultiplayerManager.PlayerCount == 3,
                "Host did not see three peers.");

            BuddhaGroup group = null;
            yield return QuantumRealmTestActions.StartSession(result => group = result);
            MultiplayerScenario.Publish("buddha-0", group.CurrentBuddhaIndexForDeviceTest.ToString());
            for (var step = 1; step <= Dwells; step++)
            {
                // Hold the hand on the Buddha and report where it was, so viewers can check the hand point.
                var hover = QuantumRealmTestActions.ActiveBuddhaHover(group);
                MultiplayerScenario.Publish($"hand-{step}", Vector(hover.CenterPosition));
                yield return QuantumRealmTestActions.DwellOnActiveBuddha(group);
                MultiplayerScenario.Publish($"buddha-{step}", group.CurrentBuddhaIndexForDeviceTest.ToString());
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, $"seen-{step}");
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, $"seen-{step}");
            }

            Object.FindAnyObjectByType<QuantumRealmUIPanel>(FindObjectsInactive.Include).OnSwitchButtonPressedFunc();
            var before = int.Parse(MultiplayerScenario.Read(MultiplayerScenario.Host, $"buddha-{Dwells}"));
            var count = group.GetComponentsInChildren<BuddhaController>(true).Length;
            yield return EditorAppDriver.WaitUntil(() => group.CurrentBuddhaIndexForDeviceTest == (before + 1) % count,
                "The Switch button did not activate the next Buddha.", 5f);
            MultiplayerScenario.Publish("buddha-switched", group.CurrentBuddhaIndexForDeviceTest.ToString());
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator ViewerRole()
        {
            var role = MultiplayerScenario.Role;
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            var type = role == MultiplayerScenario.Spectator ? HoloKitAppPlayerType.Spectator : HoloKitAppPlayerType.Player;
            yield return EditorAppDriver.EnterReality(Scene, false, type);
            yield return EditorAppDriver.WaitUntil(() => HoloKitApp.Instance.MultiplayerManager.LocalPlayer != null,
                $"The {role} did not join the host.");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());

            // Only the host can start: clients remove the hover-to-start button.
            yield return new WaitForSecondsRealtime(2f);
            Assert.That(Object.FindAnyObjectByType<HoverableStartButton>(), Is.Null, $"The {role} must not get a start button.");

            BuddhaGroup group = null;
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "buddha-0");
            yield return QuantumRealmTestActions.WaitForGroup(result => group = result);
            Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo(int.Parse(MultiplayerScenario.Read(MultiplayerScenario.Host, "buddha-0"))));
            var handPoint = Object.FindAnyObjectByType<QuantumRealmHostHandPoint>();
            Assert.That(handPoint, Is.Not.Null, $"The {role} does not have the host's hand point.");

            for (var step = 1; step <= Dwells; step++)
            {
                // While the host holds its hand on the Buddha, its hand point arrives there on this device.
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, $"hand-{step}");
                var target = ParseVector(MultiplayerScenario.Read(MultiplayerScenario.Host, $"hand-{step}"));
                var reached = false;
                var deadline = Time.realtimeSinceStartup + 10f;
                while (!reached && Time.realtimeSinceStartup < deadline)
                {
                    reached = Vector3.Distance(handPoint.transform.position, target) < 0.7f;
                    yield return null;
                }
                Assert.That(reached, Is.True, $"Step {step}: the host's hand point did not reach the Buddha on the {role}.");

                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, $"buddha-{step}");
                var expected = int.Parse(MultiplayerScenario.Read(MultiplayerScenario.Host, $"buddha-{step}"));
                yield return EditorAppDriver.WaitUntil(() => group.CurrentBuddhaIndexForDeviceTest == expected,
                    $"Step {step}: the {role} did not switch to Buddha {expected}.", 10f);
                MultiplayerScenario.Publish($"seen-{step}");
            }

            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "buddha-switched");
            var switched = int.Parse(MultiplayerScenario.Read(MultiplayerScenario.Host, "buddha-switched"));
            yield return EditorAppDriver.WaitUntil(() => group.CurrentBuddhaIndexForDeviceTest == switched,
                $"The {role} did not follow the host's Switch.", 10f);
            if (role == MultiplayerScenario.Spectator)
            {
                var stats = EditorAppDriver.RenderMainCamera(Path.Combine(EditorAppDriver.OutputDirectory("QuantumRealm"), "spectator.png"));
                Assert.That(stats.Magenta, Is.LessThan(0.002f), "The spectator view renders error-shader magenta.");
            }
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static string Vector(Vector3 v) =>
            string.Join(",", v.x.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
                v.y.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
                v.z.ToString("R", System.Globalization.CultureInfo.InvariantCulture));

        private static Vector3 ParseVector(string text)
        {
            var parts = text.Split(',');
            float Parse(int i) => float.Parse(parts[i], System.Globalization.CultureInfo.InvariantCulture);
            return new Vector3(Parse(0), Parse(1), Parse(2));
        }
    }
}
