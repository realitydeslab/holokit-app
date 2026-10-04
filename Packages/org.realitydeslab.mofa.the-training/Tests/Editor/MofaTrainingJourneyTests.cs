using System.Collections;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.TheTraining;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end: in the test arena a simulated person plays a full round of MOFA The Training against the AI
    /// (get ready with the Trigger button, dodge until the AI attacks, then aim and cast with Apple Watch
    /// swings, stepping aside between casts), reads the summary and starts the next round. Every step is recorded and
    /// compared with Tests/Editor/Baselines/MofaTrainingJourney.json.
    /// </summary>
    public sealed class MofaTrainingJourneyTests
    {
        private const string Scenario = "MofaTrainingJourney";
        // The round is shortened from 80 s on the host; it still ends by time, as in the game.
        private const float RoundSeconds = 40f;
        private const float StepAside = 0.8f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator APlayerFightsTheAIForAFullRound()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Journey());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Journey()
        {
            yield return EditorAppDriver.EnterReality("MOFATheTraining", true, HoloKitAppPlayerType.Player);
            var person = SimulatedPerson.Spawn();
            using var trace = new E2ETrace(Scenario);
            var manager = MofaTestActions.Manager;
            var placement = Object.FindAnyObjectByType<ARPlacementManager>();
            MofaPlayer host = null;
            MofaAIPlayer ai = null;
            trace.Probe("phase", () => manager.CurrentPhase.Value);
            trace.Probe("round", () => manager.RoundCount.Value);
            trace.Probe("hostHitTheAI", () => host != null && host.HitCount.Value > 0);
            trace.StartRecording();
            trace.Event("entered");

            yield return EditorAppDriver.WaitUntilRendering(() => placement != null && placement.IsValid,
                "The placement indicator for the AI did not appear.", 5f);
            trace.Event("placement ready");

            manager.RoundDuration = RoundSeconds;
            MofaTestActions.TapTrigger();
            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 5f);
            host = manager.LocalMofaPlayer;
            ai = Object.FindAnyObjectByType<MofaAIPlayer>();
            Assert.That(ai, Is.Not.Null, "The AI opponent did not spawn.");
            trace.Event("countdown");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
            trace.Event("fighting");

            var start = person.Phone.position;
            var side = Vector3.Cross(Vector3.up, Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up)).normalized;
            yield return person.LookAt(MofaTestActions.ShieldCentre(ai));
            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the AI", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The AI renders error-shader magenta.");
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The player does not see the AI opponent.");

            // First dodge without casting until the AI attacks, stepping from side to side while watching it.
            for (var step = 0; ai.CastCount.Value == 0 && step < 12; step++)
            {
                yield return person.WalkTo(start + side * (step % 2 == 0 ? StepAside : -StepAside));
                yield return person.LookAt(MofaTestActions.ShieldCentre(ai));
                yield return EditorAppDriver.RenderMainCameraFor(0.6f);
            }
            Assert.That(ai.CastCount.Value, Is.GreaterThan(0), "The AI did not attack while the player dodged.");
            trace.Event("AI attacked");

            // Then fight back: aim at the AI's shield and cast, then step to the other side.
            var casts = 0;
            var firstHit = false;
            for (var step = 0; MofaTestActions.Phase == MofaPhase.Fighting; step++)
            {
                var cast = false;
                yield return MofaTestActions.CastAt(person, () => MofaTestActions.ShieldCentre(ai), result => cast = result);
                if (cast) casts++;
                if (!firstHit && host.HitCount.Value > 0)
                {
                    firstHit = true;
                    trace.Event("hit the AI");
                }
                if (MofaTestActions.Phase == MofaPhase.Fighting)
                    yield return person.WalkTo(start + side * (step % 2 == 0 ? StepAside : -StepAside));
            }
            Assert.That(firstHit, Is.True, $"{casts} spells aimed at the AI never hit it.");

            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundOver, 5f);
            trace.Event("round over");
            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundResult, 6f);
            trace.Event("round result");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Waiting, 6f);
            yield return EditorAppDriver.WaitUntilRendering(() => GameObject.Find("Summary Board(Clone)") != null,
                "The summary board did not appear after the round.", 3f);
            trace.Event("summary");

            // The summary agrees with what happened.
            Assert.That(host.CastCount.Value, Is.EqualTo(casts), "The player's cast count is not the number of spells cast.");
            Assert.That(host.HitCount.Value, Is.InRange(1, casts), "Basic spells hit at most once each.");
            Assert.That(ai.Death.Value, Is.EqualTo(host.Kill.Value), "The AI's deaths are not the player's kills.");
            Assert.That(host.Death.Value, Is.EqualTo(ai.Kill.Value), "The player's deaths are not the AI's kills.");
            MofaTestActions.AssertConsistentResult(host, ai);
            Debug.Log($"[E2E] {Scenario} player {MofaTestActions.Stats(host)}; AI {MofaTestActions.Stats(ai)}");

            // Next round: Trigger again starts another countdown.
            MofaTestActions.TapTrigger();
            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 5f);
            trace.Event("next round");

            trace.AssertNoErrors();
            trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-training", Scenario));
        }
    }
}
