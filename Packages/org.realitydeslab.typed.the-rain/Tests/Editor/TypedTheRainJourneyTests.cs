using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the cloud floats 1.5 m above the host and the
    /// rain falls around the host's head, also after the host walks 3 m; spectators, one watching from the start and
    /// one joining later, see the host's head and the cloud where the host does. (The rain's floor height needs a
    /// Floor-classified plane, which XR Simulation never reports; that is a device check.)
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheRainJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheRainJourneyTests
    {
        private static readonly Vector3 CloudAboveHead = new(0f, 1.5f, 0f);
        private const float Settle = 3f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheCloudRainsOnTheHostWhereverItWalks()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheRain", "TypedTheRainJourney",
                "org.realitydeslab.typed.the-rain", null, Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static Vector3 HostEye => SpectatedJourney.Find("Host Center Eye Pose").transform.position;
        private static Vector3 Cloud => SpectatedJourney.Find("The Rain").transform.position;
        private static Vector3 RainHead => SpectatedJourney.Find("Text Rain").GetComponent<VisualEffect>().GetVector3("Head Position_position");

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            yield return EditorAppDriver.RenderMainCameraFor(Settle);
            AssertItRainsOnThePlayer(person);
            // Look up at the cloud at a slant: a phone cannot point straight up (XR Simulation limits pitch to 85 deg).
            yield return person.LookAt(Cloud + Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up).normalized);
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the cloud", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The cloud renders error-shader magenta.");
            journey.Trace.Event("cloud over the host");

            yield return person.WalkTo(journey.Start + new Vector3(0f, 0f, -3f));
            yield return EditorAppDriver.RenderMainCameraFor(Settle);
            AssertItRainsOnThePlayer(person);
            journey.Trace.Event("cloud followed the host");

            yield return journey.Share("host eye", HostEye);
            yield return journey.Share("cloud", Cloud, final: true);
        }

        private static void AssertItRainsOnThePlayer(SimulatedPerson person)
        {
            var head = person.Phone.position;
            Assert.That(Vector3.Distance(HostEye, head), Is.LessThan(0.05f), $"'Host Center Eye Pose' is at {HostEye}, not at the host's head {head}.");
            Assert.That(Vector3.Distance(Cloud, head + CloudAboveHead), Is.LessThan(0.15f), $"The cloud {Cloud} is not 1.5 m above the host's head {head}.");
            Assert.That(Vector3.Distance(RainHead, head), Is.LessThan(0.05f), $"The rain falls around {RainHead}, not the host's head {head}.");
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            if (!journey.IsLate) yield return journey.Expect("host eye", () => HostEye, 0.05f);
            yield return journey.Expect("cloud", () => Cloud, 0.15f);
            journey.Trace.Event("sees the cloud over the host");
        }
    }
}
