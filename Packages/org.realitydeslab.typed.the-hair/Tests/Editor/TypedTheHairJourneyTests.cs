using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): the hair grows from a cap on the host's head
    /// and its strands stay with the head when the host walks 3 m and turns; spectators, one watching from the start
    /// and one joining later, see the cap on the host's head.
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheHairJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheHairJourneyTests
    {
        private static readonly Vector3 CapOnHead = new(0f, 0.043f, -0.088f);   // the cap's place on "Host Center Eye Pose"
        private const float StrandRootsNearHead = 0.3f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheHairStaysOnTheHostsHead()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheHair", "TypedTheHairJourney",
                "org.realitydeslab.typed.the-hair", null, Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static Transform HostEye => SpectatedJourney.Find("Host Center Eye Pose").transform;
        private static Vector3 Cap => SpectatedJourney.Find("Cap").transform.position;

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            AssertTheHairIsOnTheHead(person);
            journey.Trace.Event("hair on the host");

            yield return person.WalkTo(journey.Start + new Vector3(0f, 0f, -3f));
            yield return person.LookAt(person.Phone.position + Vector3.right);
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            AssertTheHairIsOnTheHead(person);
            journey.Trace.Event("hair moved with the host");
            yield return journey.Share("cap", Cap, final: true);
        }

        private static void AssertTheHairIsOnTheHead(SimulatedPerson person)
        {
            var head = person.Phone;
            Assert.That(Vector3.Distance(HostEye.position, head.position), Is.LessThan(0.05f), "'Host Center Eye Pose' is not at the host's head.");
            Assert.That(Vector3.Distance(Cap, HostEye.TransformPoint(CapOnHead)), Is.LessThan(0.01f), "The cap is not on the host's head.");
            // The strands' root positions are written to the hair effect's PositionMap, one row per strand.
            var map = SpectatedJourney.Find("Wig VFX").GetComponent<VisualEffect>().GetTexture("PositionMap") as Texture2D;
            Assert.That(map, Is.Not.Null, "The hair effect has no PositionMap.");
            for (var row = 0; row < map.height; row++)
            {
                var root = (Vector4)map.GetPixel(0, row);
                Assert.That(Vector3.Distance(root, head.position), Is.LessThan(StrandRootsNearHead),
                    $"Strand {row} grows from {(Vector3)root}, not from the host's head {head.position}.");
            }
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            yield return journey.Expect("cap", () => Cap, 0.05f);
            journey.Trace.Event("sees the hair on the host");
        }
    }
}
