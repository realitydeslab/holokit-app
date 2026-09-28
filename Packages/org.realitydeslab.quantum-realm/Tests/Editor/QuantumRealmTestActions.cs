using System;
using System.Collections;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using RealityDesignLab.QuantumRealm;
using UnityEngine;

namespace HoloKit.Tests
{
    /// <summary>Host actions in Quantum Realm, shared by the single-Editor and multiplayer tests.</summary>
    internal static class QuantumRealmTestActions
    {
        public const float DwellTimeout = 6f;

        /// <summary>Waits for the hover-to-start button, presses the Star mode Trigger and waits for the Buddhas.</summary>
        public static IEnumerator StartSession(Action<BuddhaGroup> done)
        {
            yield return EditorAppDriver.WaitUntil(
                () => UnityEngine.Object.FindAnyObjectByType<HoverableStartButton>() is { isActiveAndEnabled: true },
                "The hover-to-start button did not appear.");
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            yield return WaitForGroup(done);
        }

        public static IEnumerator WaitForGroup(Action<BuddhaGroup> done)
        {
            yield return EditorAppDriver.WaitUntil(() => UnityEngine.Object.FindAnyObjectByType<BuddhaGroup>() is { IsSpawned: true },
                "The Buddha group did not spawn.");
            yield return new WaitForSecondsRealtime(1.5f);
            done(UnityEngine.Object.FindAnyObjectByType<BuddhaGroup>());
        }

        public static HoverableObject ActiveBuddhaHover(BuddhaGroup group)
        {
            var active = group.GetComponentsInChildren<BuddhaController>(false)
                .Select(buddha => buddha.GetComponent<HoverableObject>())
                .Where(hover => hover != null && hover.isActiveAndEnabled).ToList();
            Assert.That(active, Has.Count.EqualTo(1), "Exactly one Buddha should be shown.");
            return active[0];
        }

        /// <summary>Injects a synthetic hand pose; all 21 joints at one point.</summary>
        public static void PlaceHand(Vector3 position)
        {
            HoloKitHandTracker.Instance.InjectPoseForDeviceTest(Enumerable.Repeat(position, 21).ToArray());
        }

        /// <summary>Holds the hand on the shown Buddha until the next one activates, then lifts it away.</summary>
        public static IEnumerator DwellOnActiveBuddha(BuddhaGroup group)
        {
            var index = group.CurrentBuddhaIndexForDeviceTest;
            var hover = ActiveBuddhaHover(group);
            var startedLoading = false;
            var peakLoad = 0f;
            var deadline = Time.realtimeSinceStartup + DwellTimeout;
            while (group.CurrentBuddhaIndexForDeviceTest == index && Time.realtimeSinceStartup < deadline)
            {
                PlaceHand(hover.CenterPosition);
                startedLoading |= hover.IsLoading;
                peakLoad = Mathf.Max(peakLoad, hover.CurrentLoadPercentage);
                yield return null;
            }
            Assert.That(startedLoading, Is.True, $"Buddha {index}: the hand inside its radius did not start loading.");
            var count = group.GetComponentsInChildren<BuddhaController>(true).Length;
            Assert.That(group.CurrentBuddhaIndexForDeviceTest, Is.EqualTo((index + 1) % count),
                $"Buddha {index}: a full dwell (peak load {peakLoad:P0}) did not activate the next Buddha.");
            PlaceHand(hover.CenterPosition + Vector3.up * 5f);
            // The previous Buddha fades out before the next one is the only one shown.
            yield return new WaitForSecondsRealtime(1.5f);
        }
    }
}
