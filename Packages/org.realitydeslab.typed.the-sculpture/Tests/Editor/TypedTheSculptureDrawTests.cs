using System.Collections;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;
using NUnit.Framework;
using RealityDesignLab.Typed.TheSculpture;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;
using HoloKit;

namespace HoloKit.Tests
{
    /// <summary>
    /// The Sculpture draws when the host looks at it from where it started it, from a step away (on iPhone its
    /// close-up drew nothing) and after reaching out with a hand. Started with Trigger, so no hand is needed to start.
    /// </summary>
    public sealed class TypedTheSculptureDrawTests
    {
        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(300000)]
        public IEnumerator TheSculptureDrawsWhenTheHostLooksAtIt()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Draws());
            yield return new ExitPlayMode();
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator TheSculptureStartedWithAHandDrawsUpClose()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(StartedWithAHand());
            yield return new ExitPlayMode();
        }

        // As a host starts it: a hand held on the floating start button, then lowered; then walking up to it.
        private static IEnumerator StartedWithAHand()
        {
            yield return EditorAppDriver.EnterReality("TypedTheSculpture", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace("TypedTheSculptureHandStart");
            var button = SpectatedJourney.Find("Hoverable Start Button");
            yield return EditorAppDriver.WaitUntilRendering(() => button.activeInHierarchy, "The start button did not appear.", 5f);
            yield return person.HoldHandOn(() => button.transform.position, () => Object.FindAnyObjectByType<StickerManController>() != null, 8f,
                "Holding a hand on the start button did not start the sculpture.");
            yield return person.LowerHand();
            var sculpture = Object.FindAnyObjectByType<StickerManController>();
            var vfx = sculpture.GetComponentInChildren<VisualEffect>();
            var toward = Vector3.ProjectOnPlane(sculpture.transform.position - person.Phone.position, Vector3.up).normalized;
            yield return person.WalkTo(sculpture.transform.position - toward * 1.2f);
            yield return person.LookAt(sculpture.transform.position + Vector3.up * 0.8f);
            yield return EditorAppDriver.RenderMainCameraFor(2f);
            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the sculpture up close", stats => view = stats);
            var hand = ((TypedTheSculptureRealityManager)HoloKitApp.Instance.RealityManager).HostHandPose;
            var state = $"alive {vfx.aliveParticleCount}; hand at {hand.transform.position} active {hand.IsActive}; " +
                        $"interactable {vfx.GetBool("IsInteractable")} vfx hand {vfx.GetVector3("Hand")}";
            Debug.Log("[E2E] started with a hand: " + state);
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The sculpture started with a hand does not draw up close: " + state);
        }

        /// <summary>
        /// The figure fills with letters launched from the host's hand: reaching into it fills it, withdrawing the hand
        /// (out of the camera's view, so no longer tracked) lets it empty as the letters' lifetime (4-9 s) runs out,
        /// and reaching back in fills it again. The Editor hand is tracked only while the camera sees it, as on a device.
        /// </summary>
        [UnityTest, Timeout(300000)]
        public IEnumerator TheFigureFillsFromTheHostsHand()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(FillsFromTheHand());
            yield return new ExitPlayMode();
        }

        private const float LettersLifetime = 9f;

        private static IEnumerator FillsFromTheHand()
        {
            yield return EditorAppDriver.EnterReality("TypedTheSculpture", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace("TypedTheSculptureFills");
            var button = SpectatedJourney.Find("Hoverable Start Button");
            yield return EditorAppDriver.WaitUntilRendering(() => button.activeInHierarchy, "The start button did not appear.", 5f);
            yield return person.HoldHandOn(() => button.transform.position, () => Object.FindAnyObjectByType<StickerManController>() != null, 8f,
                "Holding a hand on the start button did not start the sculpture.");
            var sculpture = Object.FindAnyObjectByType<StickerManController>();
            var vfx = sculpture.GetComponentInChildren<VisualEffect>();
            var figure = sculpture.transform.position + Vector3.up * 0.8f;
            var toward = Vector3.ProjectOnPlane(figure - person.Phone.position, Vector3.up).normalized;
            yield return person.WalkTo(sculpture.transform.position - toward * 1.2f);
            yield return person.LookAt(figure);

            // Reach into the figure, pointing, and hold the hand there.
            yield return person.RaiseHandTo(person.Phone.position + (figure - person.Phone.position).normalized * 0.6f, shape: SimulatedPerson.HandShape.Point);
            yield return EditorAppDriver.RenderMainCameraFor(4f);
            var filled = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("reaching in", stats => filled = stats);
            Assert.That(vfx.GetBool("IsInteractable"), Is.True, "The sculpture does not take the tracked hand.");
            Assert.That(filled.Lit, Is.GreaterThan(0.01f), "Reaching into the figure did not fill it with letters.");

            // Withdraw the hand: out of view it is no longer tracked, no letters are launched, and the figure empties.
            yield return person.LowerHand();
            yield return EditorAppDriver.WaitUntilRendering(() => !HoloKitHandTracker.Instance.IsValid, "The lowered hand is still tracked.", 2f);
            yield return EditorAppDriver.RenderMainCameraFor(LettersLifetime + 1f);
            var emptied = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("hand withdrawn", stats => emptied = stats);
            Assert.That(vfx.GetBool("IsInteractable"), Is.False, "Without a tracked hand the sculpture still launches letters.");
            Assert.That(emptied.Lit, Is.LessThan(filled.Lit * 0.2f), "The figure did not empty once the hand was withdrawn.");

            // Reach back in: it fills again.
            yield return person.RaiseHandTo(person.Phone.position + (figure - person.Phone.position).normalized * 0.6f, shape: SimulatedPerson.HandShape.Point);
            yield return EditorAppDriver.RenderMainCameraFor(4f);
            var refilled = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("reaching in again", stats => refilled = stats);
            Assert.That(refilled.Lit, Is.GreaterThan(0.01f), "Reaching back into the figure did not fill it again.");
            Debug.Log($"[E2E] figure lit: filled {filled.Lit:0.000}, withdrawn {emptied.Lit:0.000}, refilled {refilled.Lit:0.000}");
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Draws()
        {
            yield return EditorAppDriver.EnterReality("TypedTheSculpture", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace("TypedTheSculptureDraw");
            var button = SpectatedJourney.Find("Hoverable Start Button");
            yield return EditorAppDriver.WaitUntilRendering(() => button.activeInHierarchy, "The start button did not appear.", 5f);
            HoloKitAppUIEventManager.OnStarUITriggered?.Invoke();
            yield return EditorAppDriver.WaitUntilRendering(() => Object.FindAnyObjectByType<StickerManController>() != null,
                "Trigger did not start the sculpture.", 5f);
            var sculpture = Object.FindAnyObjectByType<StickerManController>();
            var vfx = sculpture.GetComponentInChildren<VisualEffect>();
            yield return person.LookAt(sculpture.transform.position + Vector3.up * 0.8f);
            yield return EditorAppDriver.RenderMainCameraFor(2f);
            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the sculpture", stats => view = stats);
            var state = $"sculpture at {sculpture.transform.position}, phone at {person.Phone.position} looking {person.Phone.forward}; " +
                        $"effect '{vfx.visualEffectAsset?.name}' enabled {vfx.enabled} alive {vfx.aliveParticleCount} culled {vfx.culled} " +
                        $"bounds {vfx.GetComponent<Renderer>()?.bounds}";
            Debug.Log("[E2E] " + state);
            Assert.That(vfx.aliveParticleCount, Is.GreaterThan(0), "The sculpture has no particles: " + state);
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The host does not see the sculpture: " + state);

            // Up close, a step away, as a person walks up to it.
            var toward = Vector3.ProjectOnPlane(sculpture.transform.position - person.Phone.position, Vector3.up).normalized;
            yield return person.WalkTo(sculpture.transform.position - toward * 1.2f);
            yield return person.LookAt(sculpture.transform.position + Vector3.up * 0.8f);
            yield return EditorAppDriver.RenderMainCameraFor(2f);
            yield return trace.CapturePlayerView("the sculpture up close", stats => view = stats);
            state = $"sculpture at {sculpture.transform.position}, phone at {person.Phone.position} looking {person.Phone.forward}; " +
                    $"alive {vfx.aliveParticleCount} culled {vfx.culled} bounds {vfx.GetComponent<Renderer>()?.bounds}";
            Debug.Log("[E2E] up close: " + state);
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The host does not see the sculpture up close: " + state);

            // From the side, looking along world X: every letter must face the viewer, not a fixed direction.
            yield return person.WalkTo(sculpture.transform.position - Vector3.right * 1.2f);
            yield return person.LookAt(sculpture.transform.position + Vector3.up * 0.8f);
            yield return EditorAppDriver.RenderMainCameraFor(2f);
            yield return trace.CapturePlayerView("the sculpture from the side", stats => view = stats);
            Debug.Log($"[E2E] from the side: lit {view.LitCenter} looking {person.Phone.forward}");
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), $"From the side (looking {person.Phone.forward}) the sculpture's letters are edge-on.");

            // And after the host reached out and lowered the hand to its side (the sculpture then follows the hand).
            yield return person.RaiseHandTo(person.Phone.position + person.Phone.forward * 0.5f);
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            yield return person.LowerHand();
            yield return person.LookAt(sculpture.transform.position + Vector3.up * 0.8f);
            yield return EditorAppDriver.RenderMainCameraFor(2f);
            yield return trace.CapturePlayerView("after the hand was lowered", stats => view = stats);
            var hand = ((TypedTheSculptureRealityManager)HoloKitApp.Instance.RealityManager).HostHandPose;
            state = $"hand at {hand.transform.position} active {hand.IsActive}; interactable {vfx.GetBool("IsInteractable")} vfx hand {vfx.GetVector3("Hand")}; " +
                    $"alive {vfx.aliveParticleCount} lit {view.LitCenter}";
            Debug.Log("[E2E] after the hand: " + state);
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The sculpture vanished after the hand was lowered: " + state);
        }
    }
}
