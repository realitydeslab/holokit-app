using System.Collections;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.Typed.TheSculpture;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the host holds a hand on the floating start
    /// button; the sculpture appears at the placement point; the host walks up to it and reaches into it, and the
    /// sculpture follows the hand. Spectators, one watching from the start and one joining later, see the sculpture
    /// and the host's hand where the host does.
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheSculptureJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheSculptureJourneyTests
    {
        private const float StandOff = 1.2f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheHostStartsTheSculptureAndReachesIntoIt()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheSculpture", "TypedTheSculptureJourney",
                "org.realitydeslab.typed.the-sculpture", trace => trace.Probe("sculptures", () => Object.FindObjectsByType<StickerManController>(FindObjectsSortMode.None).Length),
                Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static StickerManController Sculpture => Object.FindAnyObjectByType<StickerManController>();
        private static Transform Hand => ((TypedTheSculptureRealityManager)HoloKitApp.Instance.RealityManager).HostHandPose.transform;
        private static VisualEffect SculptureVfx => Sculpture.GetComponentInChildren<VisualEffect>();

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            var button = SpectatedJourney.Find("Hoverable Start Button");
            yield return EditorAppDriver.WaitUntilRendering(() => button.activeInHierarchy, "The start button did not appear.", 5f);
            journey.Trace.Event("start button");
            yield return person.HoldHandOn(() => button.transform.position, () => Sculpture != null, 8f,
                "Holding a hand on the start button did not start the sculpture.");
            yield return person.LowerHand();
            journey.Trace.Event("sculpture started");

            // Walk up to the sculpture and reach into it.
            var sculpture = Sculpture.transform.position;
            var toward = Vector3.ProjectOnPlane(sculpture - person.Phone.position, Vector3.up).normalized;
            yield return person.WalkTo(sculpture - toward * StandOff);
            yield return person.LookAt(sculpture + Vector3.up * 0.8f);
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the sculpture", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The sculpture renders error-shader magenta.");
            // Whether it is visible is checked in one Editor (TypedTheSculptureDrawTests): with three Editors running,
            // this capture draws no particles even though the effect has them, is not culled and occlusion is off.
            yield return person.RaiseHandTo(person.Phone.position + person.Phone.forward * 0.55f);
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            AssertTheSculptureFollowsTheHand();
            journey.Trace.Event("reached into the sculpture");

            yield return journey.Share("sculpture", sculpture, final: true);
            yield return journey.Share("hand", Hand.position, final: true);
        }

        private static void AssertTheSculptureFollowsTheHand()
        {
            Assert.That(SculptureVfx.GetBool("IsInteractable"), Is.True, "The sculpture does not react to the hand.");
            Assert.That(Vector3.Distance(SculptureVfx.GetVector3("Hand"), Hand.position), Is.LessThan(0.03f),
                "The sculpture follows a point away from the host's hand.");
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            yield return EditorAppDriver.WaitUntilRendering(() => Sculpture != null, "The spectator does not see the sculpture.", 30f);
            yield return journey.Expect("sculpture", () => Sculpture.transform.position, 0.05f);
            yield return journey.Expect("hand", () => Hand.position, 0.05f);
            yield return EditorAppDriver.RenderMainCameraFor(0.5f);
            AssertTheSculptureFollowsTheHand();
            journey.Trace.Event("sees the sculpture follow the hand");
        }
    }
}
