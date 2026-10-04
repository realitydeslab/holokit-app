using System.Collections;
using System.Linq;
using NUnit.Framework;
using BoidsSimulationOnGPU;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the flock circles the host 1.5 m away and keeps
    /// circling the host after it walks 3 m; spectators, one watching from the start and one joining later, see the
    /// host's head and the flock where the host does.
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheFlockJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheFlockJourneyTests
    {
        private const float Orbit = 1.5f;         // "RoateAround" sits 1.5 m from "Boid Target"
        private const float Settle = 3f;          // the target follows the head with a soft follow
        private const float FlockRadius = 0.75f;  // GPUBoids keeps the birds within this sphere around "Boid"

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheFlockCirclesTheHostWhereverItWalks()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheFlock", "TypedTheFlockJourney",
                "org.realitydeslab.typed.the-flock", null, Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static Vector3 HostEye => SpectatedJourney.Find("Host Center Eye Pose").transform.position;
        private static Vector3 Target => SpectatedJourney.Find("Boid Target").transform.position;
        private static Vector3 Boid => SpectatedJourney.Find("Boid").transform.position;

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            yield return EditorAppDriver.RenderMainCameraFor(Settle);
            AssertTheFlockCirclesThePlayer(person);
            yield return person.LookAt(Boid);
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the flock", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The flock renders error-shader magenta.");
            journey.Trace.Event("flock around the host");

            yield return person.WalkTo(journey.Start + new Vector3(0f, 0f, -3f));
            yield return EditorAppDriver.RenderMainCameraFor(Settle);
            AssertTheFlockCirclesThePlayer(person);
            journey.Trace.Event("flock followed the host");

            yield return journey.Share("host eye", HostEye);
            yield return journey.Share("boid target", Target, final: true);
        }

        private static void AssertTheFlockCirclesThePlayer(SimulatedPerson person)
        {
            Assert.That(SpectatedJourney.Flat(HostEye, person.Phone.position), Is.LessThan(0.05f),
                $"'Host Center Eye Pose' is at {HostEye}, not at the host's head {person.Phone.position}.");
            Assert.That(SpectatedJourney.Flat(Target, person.Phone.position), Is.LessThan(0.3f),
                $"The flock's centre {Target} is not with the host at {person.Phone.position}.");
            Assert.That(Vector3.Distance(Boid, Target), Is.EqualTo(Orbit).Within(0.2f), "The flock is not circling 1.5 m away.");
            var boids = Object.FindAnyObjectByType<GPUBoids>();
            var positions = new Vector3[boids.GetMaxObjectNum()];
            boids.GetBoidPositionDataBuffer().GetData(positions);
            var centre = positions.Aggregate(Vector3.zero, (sum, p) => sum + p) / positions.Length;
            // The birds chase the circling centre, so on average they fly inside the circle around the host.
            Assert.That(SpectatedJourney.Flat(centre, Target), Is.LessThan(Orbit + FlockRadius),
                $"The birds ({centre}) do not fly around the host ({Target}).");
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            if (!journey.IsLate) yield return journey.Expect("host eye", () => HostEye, 0.05f);
            yield return journey.Expect("boid target", () => Target, 0.3f);
            journey.Trace.Event("sees the flock around the host");
        }
    }
}
