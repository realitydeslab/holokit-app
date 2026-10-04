using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the host raises a hand; five ribbons start at
    /// its fingertips and follow when the hand moves; spectators, one watching from the start and one joining while
    /// the hand is up, see the ribbons at the host's fingertips.
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheFingerRibbonJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheFingerRibbonJourneyTests
    {
        private static readonly string[] Fingers = { "Thumb", "Index", "Middle", "Ring", "Little" };
        private static readonly string[] Ribbons = { "Thumb", "Index", "Middle", "Ring", "Pinky" };   // "Finger Lines_<name>"
        private static readonly HandJoint[] Tips = { HandJoint.ThumbTip, HandJoint.IndexTip, HandJoint.MiddleTip, HandJoint.RingTip, HandJoint.LittleTip };
        private const float Follow = 5f;     // the soft tips catch up with the fingertips (lerp 4/s; slower game time under load)

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator RibbonsFlowFromTheHostsFingertips()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheFingerRibbon", "TypedTheFingerRibbonJourney",
                "org.realitydeslab.typed.the-finger-ribbon", trace => trace.Probe("ribbons", () => RibbonsShown), Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static bool RibbonsShown => SpectatedJourney.Find("Finger VFX Manager").activeInHierarchy;
        private static Vector3 SoftTip(int finger) => SpectatedJourney.Find("GameObject_" + Fingers[finger]).transform.position;
        private static Vector3 Ribbon(int finger) => SpectatedJourney.Find("Finger Lines_" + Ribbons[finger]).transform.position;

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            Assert.That(RibbonsShown, Is.False, "Ribbons must wait for a hand.");
            var hand = person.Phone.position + person.Phone.forward * 0.45f - Vector3.up * 0.1f;
            yield return person.RaiseHandTo(hand);
            yield return EditorAppDriver.WaitUntilRendering(() => RibbonsShown, "Raising a hand did not start the ribbons.", 3f);
            yield return EditorAppDriver.WaitUntilRendering(TipsCaughtUp, "The ribbons' tips did not catch up with the fingertips.", Follow);
            AssertRibbonsAtTheFingertips();
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the ribbons", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The ribbons render error-shader magenta.");
            journey.Trace.Event("ribbons at the fingertips");

            yield return person.RaiseHandTo(hand + person.Phone.right * 0.2f);
            yield return EditorAppDriver.WaitUntilRendering(TipsCaughtUp, "The ribbons did not follow the hand.", Follow);
            AssertRibbonsAtTheFingertips();
            journey.Trace.Event("ribbons followed the hand");
            // The hand stays up while the spectators check, the late one joining now.
            yield return journey.Share("index tip", SoftTip(1), final: true);
            yield return journey.Share("little tip", SoftTip(4), final: true);
        }

        private static bool TipsCaughtUp()
        {
            var tracker = HoloKitHandTracker.Instance;
            for (var finger = 0; finger < Fingers.Length; finger++)
                if (Vector3.Distance(SoftTip(finger), tracker.GetHandJointPosition(Tips[finger])) >= 0.02f) return false;
            return true;
        }

        private static void AssertRibbonsAtTheFingertips()
        {
            var tracker = HoloKitHandTracker.Instance;
            for (var finger = 0; finger < Fingers.Length; finger++)
            {
                Assert.That(Vector3.Distance(SoftTip(finger), tracker.GetHandJointPosition(Tips[finger])), Is.LessThan(0.02f),
                    $"The {Fingers[finger]} ribbon's tip {SoftTip(finger)} did not catch up with the fingertip {tracker.GetHandJointPosition(Tips[finger])} " +
                    $"({Object.FindObjectsByType<HoloKitHandTracker>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length} hand trackers).");
                Assert.That(Vector3.Distance(Ribbon(finger), SoftTip(finger)), Is.LessThan(0.01f),
                    $"The {Fingers[finger]} ribbon is not drawn at its tip.");
            }
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            yield return journey.Expect("index tip", () => SoftTip(1), 0.03f);
            yield return journey.Expect("little tip", () => SoftTip(4), 0.03f);
            yield return EditorAppDriver.WaitUntilRendering(() => RibbonsShown, "The spectator does not see the host's ribbons.", 3f);
            Assert.That(Vector3.Distance(Ribbon(1), SoftTip(1)), Is.LessThan(0.01f), "The spectator draws the index ribbon away from its tip.");
            journey.Trace.Event("sees the ribbons");
        }
    }
}
