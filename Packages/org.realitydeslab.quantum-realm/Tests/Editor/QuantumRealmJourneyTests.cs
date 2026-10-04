using System.Collections;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.QuantumRealm;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end: a simulated person in the test arena scans the room, starts Quantum Realm with a hand, then walks the circle
    /// and holds a hand on each of the ten Buddhas in turn. Every step is recorded and compared with
    /// Tests/Editor/Baselines/QuantumRealmJourney.json. The spec is in the package README.
    /// </summary>
    public sealed class QuantumRealmJourneyTests
    {
        private const string Scenario = "QuantumRealmJourney";
        // Quantum Realm touches with the index knuckle projected 0.5 m along the eye-to-hand ray
        // (QuantumRealm.unity: Extended Hand Joint, _extendedLength 0.5).
        private const float TouchExtension = 0.5f;
        private const float StandOff = 1f;        // metres from a Buddha: arm plus the extended touch reach it
        private const float DwellTimeout = 8f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator APlayerWalksTheCircleAndTouchesEveryBuddha()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Journey());
            yield return new ExitPlayMode();
        }

        private static string ActiveEffect(BuddhaGroup group)
        {
            var effect = QuantumRealmTestActions.ActiveBuddhaHover(group).GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            return effect != null && effect.visualEffectAsset != null ? effect.visualEffectAsset.name : "none";
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Journey()
        {
            yield return EditorAppDriver.EnterReality("QuantumRealm", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace(Scenario);
            BuddhaGroup group = null;
            var placement = Object.FindAnyObjectByType<ARPlacementManager>();
            trace.Probe("planeFound", () => placement != null && placement.IsValid);
            trace.Probe("started", () => group != null);
            trace.Probe("buddha", () => group == null ? -1 : group.CurrentBuddhaIndexForDeviceTest);
            trace.Probe("shown", () => group == null ? 0 : group.GetComponentsInChildren<BuddhaController>(false).Length);
            trace.Probe("effect", () => group == null ? "-" : ActiveEffect(group));
            trace.StartRecording();
            trace.Event("entered");

            // Look at the floor ahead and around until AR finds a plane to place the Buddhas on.
            var ahead = Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up).normalized;
            foreach (var turn in new[] { 0f, -40f, 40f, 0f })
            {
                if (placement.IsValid) break;
                var look = Quaternion.Euler(0f, turn, 0f) * ahead * 1.5f + Vector3.down * 1.2f;
                yield return person.LookAt(person.Phone.position + look);
                yield return EditorAppDriver.RenderMainCameraFor(1f);
            }
            Assert.That(placement.IsValid, Is.True, "Scanning the floor did not find a plane.");
            trace.Event("plane found");

            // Start as a player does: the start button floats in front of the player; reach out and hold it.
            var start = Object.FindAnyObjectByType<HoverableStartButton>();
            Assert.That(start, Is.Not.Null, "The hover-to-start button did not appear.");
            var button = start.GetComponent<HoverableObject>();
            yield return person.LookAt(button.CenterPosition);
            yield return EditorAppDriver.RenderMainCameraFor(1.5f);   // the button glides into place in front
            yield return person.HoldHandOn(() => button.CenterPosition,
                () => Object.FindAnyObjectByType<BuddhaGroup>() is { IsSpawned: true }, DwellTimeout,
                "Holding a hand on the start button did not start the session.", TouchExtension);
            yield return person.LowerHand();
            group = Object.FindAnyObjectByType<BuddhaGroup>();
            yield return EditorAppDriver.RenderMainCameraFor(1.5f);
            trace.Event("session started");

            // Walk the circle: stand in front of the shown Buddha, look at it, hold a hand on it.
            var count = group.GetComponentsInChildren<BuddhaController>(true).Length;
            var centre = group.transform.position;
            for (var step = 0; step < count; step++)
            {
                var index = group.CurrentBuddhaIndexForDeviceTest;
                var hover = QuantumRealmTestActions.ActiveBuddhaHover(group);
                var target = hover.CenterPosition;
                var outward = Vector3.ProjectOnPlane(target - centre, Vector3.up).normalized;
                yield return person.WalkTo(target - outward * StandOff);
                yield return person.LookAt(target);
                yield return EditorAppDriver.RenderMainCameraFor(1.5f);
                var phoneView = trace.Capture($"buddha-{index}-phone");
                Assert.That(phoneView.Magenta, Is.LessThan(0.002f), $"Buddha {index} renders error-shader magenta.");
                var seen = default(EditorAppDriver.RenderStats);
                yield return trace.CapturePlayerView($"buddha-{index}", stats => seen = stats);
                Assert.That(seen.Magenta, Is.LessThan(0.002f), $"Buddha {index} renders error-shader magenta.");
                Assert.That(seen.LitCenter, Is.GreaterThan(0.02f),
                    $"Buddha {index} ({ActiveEffect(group)}) draws nothing where the player looks at it.");

                yield return person.HoldHandOn(() => target, () => group.CurrentBuddhaIndexForDeviceTest != index, DwellTimeout,
                    $"Holding a hand on Buddha {index} did not bring the next one.", TouchExtension);
                Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo((index + 1) % count),
                    $"Buddha {index} was followed by {group.CurrentBuddhaIndexForDeviceTest}, not the next one.");
                yield return person.LowerHand();
                // The previous Buddha fades out before the next one is the only one shown.
                yield return EditorAppDriver.WaitUntilRendering(() => group.GetComponentsInChildren<BuddhaController>(false).Length == 1,
                    $"Buddha {index} did not fade out after the next one appeared.", 6f);
                trace.Event($"touched buddha {index}");
            }
            trace.Event("circle complete");

            trace.Save();
            trace.AssertNoErrors();
            trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.quantum-realm", Scenario));
        }
    }
}
