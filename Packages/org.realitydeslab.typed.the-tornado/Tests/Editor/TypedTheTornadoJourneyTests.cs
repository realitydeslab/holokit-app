using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the tornado of letters stands where the host
    /// stands, also after the host walks 3 m; spectators, one watching from the start and one joining later, see it
    /// there too. (Its ground height needs a Floor-classified plane, which XR Simulation never reports.)
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheTornadoJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheTornadoJourneyTests
    {
        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheTornadoStandsWhereTheHostStands()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheTornado", "TypedTheTornadoJourney",
                "org.realitydeslab.typed.the-tornado", null, Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static Vector3 Tornado => SpectatedJourney.Find("Tornado").transform.position;

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            Assert.That(SpectatedJourney.Flat(Tornado, person.Phone.position), Is.LessThan(0.05f), "The tornado is not where the host stands.");
            yield return person.LookAt(Tornado + Vector3.forward * 0.5f);
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the tornado", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The tornado renders error-shader magenta.");
            journey.Trace.Event("tornado at the host");

            yield return person.WalkTo(journey.Start + new Vector3(0f, 0f, -3f));
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            Assert.That(SpectatedJourney.Flat(Tornado, person.Phone.position), Is.LessThan(0.05f), "The tornado did not follow the host.");
            journey.Trace.Event("tornado followed the host");
            yield return journey.Share("tornado", Tornado, final: true);
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            yield return journey.Expect("tornado", () => Tornado, 0.1f);
            journey.Trace.Event("sees the tornado at the host");
        }
    }
}
