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
                yield return QuantumRealmTestActions.DwellOnActiveBuddha(group);
            Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo(first), "Ten dwells should wrap back to the first Buddha.");

            // Leaving before the dwell completes does not switch.
            var before = group.CurrentBuddhaIndexForDeviceTest;
            var partial = QuantumRealmTestActions.ActiveBuddhaHover(group);
            for (var t = 0f; t < 0.8f; t += Time.unscaledDeltaTime)
            {
                QuantumRealmTestActions.PlaceHand(partial.CenterPosition);
                yield return null;
            }
            QuantumRealmTestActions.PlaceHand(partial.CenterPosition + Vector3.up * 5f);
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
            var hover = QuantumRealmTestActions.ActiveBuddhaHover(group);
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
            yield return QuantumRealmTestActions.StartSession(done);
        }
    }
}
