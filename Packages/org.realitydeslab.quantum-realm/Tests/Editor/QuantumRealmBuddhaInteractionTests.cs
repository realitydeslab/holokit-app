using System.Collections;
using System.IO;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using RealityDesignLab.QuantumRealm;
using RealityDesignLab.QuantumRealm.UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// Quantum Realm host interactions in Editor Play Mode (XR Simulation), with a synthetic hand
    /// pose injected into HoloKitHandTracker. This covers everything after hand detection; it does
    /// not test camera-based hand recognition. The package README lists the interaction spec.
    /// </summary>
    public sealed class QuantumRealmBuddhaInteractionTests
    {
        private const float DwellTimeout = 6f;

        [UnityTest]
        public IEnumerator HostHandDwellActivatesEveryBuddhaInTurn()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Interactions();
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator ActivatedBuddhaIsVisibleToThePlayer()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Visibility();
            yield return new ExitPlayMode();
        }

        // Coroutines below run after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Interactions()
        {
            BuddhaGroup group = null;
            yield return StartSession(result => group = result);
            var count = group.GetComponentsInChildren<BuddhaController>(true).Length;
            Assert.That(count, Is.EqualTo(10), "The Buddha group should hold 10 Buddhas.");

            // A hand dwell on each Buddha activates the next one, wrapping after the last.
            var first = group.CurrentBuddhaIndexForDeviceTest;
            for (var step = 0; step < count; step++)
            {
                var index = group.CurrentBuddhaIndexForDeviceTest;
                var hover = ActiveBuddhaHover(group);
                var startedLoading = false;
                var peakLoad = 0f;
                for (var t = 0f; group.CurrentBuddhaIndexForDeviceTest == index && t < DwellTimeout; t += Time.unscaledDeltaTime)
                {
                    PlaceHand(hover.CenterPosition);
                    startedLoading |= hover.IsLoading;
                    peakLoad = Mathf.Max(peakLoad, hover.CurrentLoadPercentage);
                    yield return null;
                }
                Assert.That(startedLoading, Is.True, $"Buddha {index}: the hand inside its radius did not start loading.");
                Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo((index + 1) % count),
                    $"Buddha {index}: a full dwell (peak load {peakLoad:P0}) did not activate the next Buddha.");
                PlaceHand(hover.CenterPosition + Vector3.up * 5f);
                // The previous Buddha fades out before the next one is the only one shown.
                yield return new WaitForSecondsRealtime(1.5f);
            }
            Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo(first), "Ten dwells should wrap back to the first Buddha.");

            // Leaving before the dwell completes does not switch.
            var before = group.CurrentBuddhaIndexForDeviceTest;
            var partial = ActiveBuddhaHover(group);
            for (var t = 0f; t < 0.8f; t += Time.unscaledDeltaTime)
            {
                PlaceHand(partial.CenterPosition);
                yield return null;
            }
            PlaceHand(partial.CenterPosition + Vector3.up * 5f);
            yield return new WaitForSecondsRealtime(3f);
            Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo(before), "A short hover must not switch the Buddha.");
            Assert.That(partial.IsLoading, Is.False, "Loading must stop once the hand leaves.");

            // The Switch button changes the Buddha without a hand.
            Object.FindAnyObjectByType<QuantumRealmUIPanel>(FindObjectsInactive.Include).OnSwitchButtonPressedFunc();
            yield return EditorAppDriver.WaitUntil(() => group.CurrentBuddhaIndexForDeviceTest == (before + 1) % count,
                "The Switch button did not activate the next Buddha.", 5f);
        }

        private static IEnumerator Visibility()
        {
            BuddhaGroup group = null;
            yield return StartSession(result => group = result);
            var output = EditorAppDriver.OutputDirectory("QuantumRealmBuddhas");
            var hover = ActiveBuddhaHover(group);
            // The group spawns around the player; turn to face the Buddha as a player would.
            yield return EditorAppDriver.FaceMainCameraTowards(hover.CenterPosition);
            var with = Path.Combine(output, "buddha-facing.png");
            var without = Path.Combine(output, "buddha-facing-hidden.png");
            // Freeze animation (the seat's ring text rotates) so only the toggled effect differs.
            var timeScale = Time.timeScale;
            Time.timeScale = 0f;
            for (var frame = 0; frame < 3; frame++) yield return new WaitForEndOfFrame();
            var stats = EditorAppDriver.RenderMainCamera(with);
            Assert.That(stats.Magenta, Is.LessThan(0.002f), "The Buddha renders error-shader magenta.");
            // Render the same view without this Buddha's effects; the difference is the Buddha itself.
            var effects = hover.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>().Where(v => v.enabled).ToList();
            effects.ForEach(v => v.enabled = false);
            for (var frame = 0; frame < 5; frame++) yield return new WaitForEndOfFrame();
            EditorAppDriver.RenderMainCamera(without);
            effects.ForEach(v => v.enabled = true);
            Time.timeScale = timeScale;
            var buddhaPixels = EditorAppDriver.DifferentPixelFraction(with, without);
            Assert.That(buddhaPixels, Is.GreaterThan(0.005f),
                $"The active Buddha is not visible when the player faces it: it changes {buddhaPixels:P2} of the view " +
                $"({effects.Count} visual effect(s) toggled). Compare {with} with {without}.");
        }

        private static IEnumerator StartSession(System.Action<BuddhaGroup> done)
        {
            yield return EditorAppDriver.EnterReality("QuantumRealm", true, HoloKitAppPlayerType.Player);
            // Once the start button shows, the Star mode Trigger starts the session without a hand.
            yield return EditorAppDriver.WaitUntil(
                () => Object.FindAnyObjectByType<HoverableStartButton>() is { isActiveAndEnabled: true },
                "The hover-to-start button did not appear.");
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            yield return EditorAppDriver.WaitUntil(() => Object.FindAnyObjectByType<BuddhaGroup>() is { IsSpawned: true },
                "The Trigger button did not spawn the Buddha group.");
            yield return new WaitForSecondsRealtime(1.5f);
            done(Object.FindAnyObjectByType<BuddhaGroup>());
        }

        private static HoverableObject ActiveBuddhaHover(BuddhaGroup group)
        {
            var active = group.GetComponentsInChildren<BuddhaController>(false)
                .Select(buddha => buddha.GetComponent<HoverableObject>())
                .Where(hover => hover != null && hover.isActiveAndEnabled).ToList();
            Assert.That(active, Has.Count.EqualTo(1), "Exactly one Buddha should be shown.");
            return active[0];
        }

        private static void PlaceHand(Vector3 position)
        {
            HoloKitHandTracker.Instance.InjectPoseForDeviceTest(Enumerable.Repeat(position, 21).ToArray());
        }
    }
}
