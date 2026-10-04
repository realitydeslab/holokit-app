using System.Collections;
using System.Linq;
using BoidsSimulationOnGPU;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.Typed.TheFlock;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.VFX;

namespace HoloKit.Tests
{
    /// <summary>The birds the flock simulates are drawn where they fly (in one Editor; see TypedTheFlockJourneyTests).</summary>
    public sealed class TypedTheFlockDrawTests
    {
        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(300000)]
        public IEnumerator TheSimulatedBirdsAreDrawn()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(BirdsAreDrawn());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator BirdsAreDrawn()
        {
            yield return EditorAppDriver.EnterReality("TypedTheFlock", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace("TypedTheFlockDraw");
            yield return EditorAppDriver.RenderMainCameraFor(3f);

            // Birds fly at up to 5 m/s, too fast to keep in view while the capture camera warms up; draw one moment of
            // the simulation, held still, and look at its densest part.
            Object.FindAnyObjectByType<BoidTypoRealityManager>().enabled = false;   // it hands the live buffers to the effect
            var boids = Object.FindAnyObjectByType<GPUBoids>();
            var positions = new Vector3[boids.GetMaxObjectNum()];
            boids.GetBoidPositionDataBuffer().GetData(positions);
            using var moment = new GraphicsBuffer(GraphicsBuffer.Target.Structured, positions.Length, 12);
            moment.SetData(positions);
            SpectatedJourney.Find("Boid").GetComponent<VisualEffect>().SetGraphicsBuffer("PositionDataBuffer", moment);
            var densest = positions.OrderByDescending(p => positions.Count(q => (q - p).sqrMagnitude < 0.09f)).First();
            yield return person.LookAt(densest);

            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the birds", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The flock renders error-shader magenta.");
            Assert.That(view.Lit, Is.GreaterThan(0.002f), $"The birds around {densest} are not drawn.");
        }
    }
}
