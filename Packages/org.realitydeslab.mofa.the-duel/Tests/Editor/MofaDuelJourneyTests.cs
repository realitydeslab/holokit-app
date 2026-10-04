using System;
using System.Collections;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors: in the test arena two simulated people walk to opposite ends and duel a full
    /// round of MOFA The Duel while a third watches from the side. The players get ready with the Trigger button,
    /// take turns aiming at each other's shield and casting with Apple Watch swings, stepping aside after each
    /// cast, until the shortened round ends; they read the summary board and get ready for the next round. All
    /// three must report the same stats and summary board, and each role's events are compared with
    /// Tests/Editor/Baselines/MofaDuelJourney-&lt;role&gt;.json.
    /// Run with <c>Tools/run_multiplayer_test.sh MOFA MofaDuelJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class MofaDuelJourneyTests
    {
        private const string Scene = "MOFATheDuel";
        private const string Scenario = "MofaDuelJourney";
        // The round is shortened from 80 s on every peer; it still ends by time, as in the game.
        private const float RoundSeconds = 40f;
        private const float Apart = 2f;          // each player stands this far from the middle of the arena
        private const float StepAside = 0.6f;
        // Basic spells fly at 3-4 m/s; a player casts this long after the opponent did, so spells never meet
        // mid-air (opposing spells cancel each other).
        private const float SpellFlight = 2.5f;

        private static readonly Vector3 HostSpot = new(0f, 0f, -Apart);
        private static readonly Vector3 PlayerSpot = new(0f, 0f, Apart);
        private static readonly Vector3 SpectatorSpot = new(3f, 0f, 0f);

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TwoPlayersDuelAFullRoundWhileASpectatorWatches()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(RunRole()));
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator RunRole()
        {
            switch (MultiplayerScenario.Role)
            {
                case MultiplayerScenario.Host: return Fighter(true);
                case MultiplayerScenario.Player: return Fighter(false);
                case MultiplayerScenario.Spectator: return Spectator();
                default: throw new ArgumentException("Unknown HOLOKIT_MP_ROLE " + MultiplayerScenario.Role);
            }
        }

        private static IEnumerator Fighter(bool isHost)
        {
            if (!isHost) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, isHost, HoloKitAppPlayerType.Player);
            if (isHost) MultiplayerScenario.Publish("hosting");
            var person = SimulatedPerson.Spawn();
            var start = person.Phone.position;
            using var trace = new E2ETrace($"{Scenario}-{MultiplayerScenario.Role}");
            var manager = MofaTestActions.Manager;
            yield return EditorAppDriver.WaitUntil(() => manager.LocalMofaPlayer != null, "The local MOFA player did not spawn.");
            var me = manager.LocalMofaPlayer;
            MofaPlayer opponent = null;
            trace.Probe("phase", () => manager.CurrentPhase.Value);
            trace.Probe("round", () => manager.RoundCount.Value);
            trace.Probe("team", () => me.Team.Value);
            trace.StartRecording();
            trace.Event("entered");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());

            // Walk to this player's end of the arena and face the other end.
            var spot = start + (isHost ? HostSpot : PlayerSpot);
            var facing = start + (isHost ? PlayerSpot : HostSpot);
            yield return person.WalkTo(spot);
            yield return person.LookAt(facing);
            yield return MultiplayerScenario.Barrier("positioned");
            yield return EditorAppDriver.WaitUntilRendering(() => manager.MofaPlayerDict.Count == 2,
                "Both players, and only they, must be duelists.", 10f);
            opponent = manager.MofaPlayerDict.Values.Single(p => p != me);
            trace.Event("positioned");

            // Each player taps the on-screen Trigger button to get ready; the round starts when both are.
            manager.RoundDuration = RoundSeconds;
            MofaTestActions.TapTrigger();
            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 30f);
            trace.Event("countdown");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
            trace.Event("fighting");

            yield return person.LookAt(MofaTestActions.ShieldCentre(opponent));
            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the opponent", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The opponent renders error-shader magenta.");
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The player does not see the opponent.");

            // Take turns, host first: cast once the opponent has cast as often (host) or once more (client), and
            // only after the opponent's spell has landed. After casting, step to the other side.
            var side = Vector3.Cross(Vector3.up, facing - spot).normalized;
            var casts = 0;
            var exchanged = false;
            var lastOpponentCast = opponent.CastCount.Value;
            var opponentCastAt = float.NegativeInfinity;
            while (MofaTestActions.Phase == MofaPhase.Fighting)
            {
                if (!exchanged && me.HitCount.Value > 0 && opponent.HitCount.Value > 0)
                {
                    exchanged = true;
                    trace.Event("exchanged hits");
                }
                if (opponent.CastCount.Value != lastOpponentCast)
                {
                    lastOpponentCast = opponent.CastCount.Value;
                    opponentCastAt = Time.realtimeSinceStartup;
                }
                var myTurn = isHost ? me.CastCount.Value <= opponent.CastCount.Value : me.CastCount.Value < opponent.CastCount.Value;
                if (!myTurn || Time.realtimeSinceStartup - opponentCastAt < SpellFlight)
                {
                    yield return EditorAppDriver.RenderMainCameraFrames(1);
                    continue;
                }
                var cast = false;
                yield return MofaTestActions.CastAt(person, () => MofaTestActions.ShieldCentre(opponent), result => cast = result);
                if (cast) casts++;
                if (MofaTestActions.Phase == MofaPhase.Fighting)
                    yield return person.WalkTo(spot + side * (casts % 2 == 1 ? StepAside : -StepAside));
            }
            Assert.That(exchanged, Is.True,
                $"Both players must hit each other: me {MofaTestActions.Stats(me)}, opponent {MofaTestActions.Stats(opponent)}.");

            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundOver, 5f);
            trace.Event("round over");
            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundResult, 6f);
            trace.Event("round result");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Waiting, 6f);
            yield return EditorAppDriver.WaitUntilRendering(() => MofaTestActions.SummaryBoardText() != null,
                "The summary board did not appear after the round.", 3f);
            yield return EditorAppDriver.RenderMainCameraFor(0.5f);   // the board fills in its text on Update
            trace.Event("summary");

            // The summary agrees with what happened.
            Assert.That(me.CastCount.Value, Is.EqualTo(casts), "The player's cast count is not the number of spells cast.");
            Assert.That(me.HitCount.Value, Is.InRange(1, casts), "Basic spells hit at most once each.");
            Assert.That(me.Kill.Value, Is.EqualTo(opponent.Death.Value), "My kills are not the opponent's deaths.");
            Assert.That(me.Death.Value, Is.EqualTo(opponent.Kill.Value), "My deaths are not the opponent's kills.");
            Assert.That(me.Distance.Value, Is.GreaterThan(1f), "Stepping aside during the round did not count as distance.");
            MofaTestActions.AssertConsistentResult(me, opponent);
            yield return AgreeOnFinalState(manager);

            // Next round: both press Trigger again.
            MofaTestActions.TapTrigger();
            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 30f);
            trace.Event("next round");

            trace.AssertNoErrors();
            trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-duel", $"{Scenario}-{MultiplayerScenario.Role}"));
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator Spectator()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Spectator);
            var app = HoloKitApp.Instance;
            var person = SimulatedPerson.Spawn();
            var start = person.Phone.position;
            using var trace = new E2ETrace($"{Scenario}-{MultiplayerScenario.Role}");
            var manager = MofaTestActions.Manager;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The spectator did not join the host.");
            trace.Probe("phase", () => manager.CurrentPhase.Value);
            trace.Probe("round", () => manager.RoundCount.Value);
            trace.StartRecording();
            trace.Event("entered");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());

            // Stand at the side of the arena and watch the middle; a spectator has no Trigger button.
            yield return person.WalkTo(start + SpectatorSpot);
            yield return person.LookAt(start);
            Assert.That(EditorAppDriver.IsUiVisible("Trigger Button"), Is.False, "A spectator must not see the Trigger button.");
            yield return MultiplayerScenario.Barrier("positioned");
            yield return EditorAppDriver.WaitUntilRendering(() => manager.MofaPlayerDict.Count == 2,
                "The spectator must see two duelists and not count as one.", 10f);
            trace.Event("positioned");

            manager.RoundDuration = RoundSeconds;
            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 30f);
            trace.Event("countdown");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
            trace.Event("fighting");

            // A spectator's Watch swings cast nothing.
            var self = app.MultiplayerManager.LocalPlayer as MofaPlayer;
            for (var i = 0; i < 3; i++)
            {
                MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                yield return EditorAppDriver.RenderMainCameraFor(1f);
            }
            Assert.That(self == null || self.CastCount.Value == 0, Is.True, "A spectator must not be able to cast.");

            // Watch the host, then wait for both players to land a hit.
            var host = manager.MofaPlayerDict.Values.Single(p => p.OwnerClientId == NetworkManager.ServerClientId);
            yield return person.LookAt(MofaTestActions.ShieldCentre(host));
            var view = default(EditorAppDriver.RenderStats);
            yield return trace.CapturePlayerView("the host from the side", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The spectator view renders error-shader magenta.");
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The spectator does not see the host.");
            yield return person.LookAt(start);
            yield return EditorAppDriver.WaitUntilRendering(() => manager.MofaPlayerDict.Values.All(p => p.HitCount.Value > 0)
                || MofaTestActions.Phase != MofaPhase.Fighting, "The round did not end.", RoundSeconds + 5f);
            Assert.That(manager.MofaPlayerDict.Values.All(p => p.HitCount.Value > 0), Is.True, "The spectator did not see both players hit.");
            trace.Event("exchanged hits");

            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundOver, RoundSeconds + 5f);
            trace.Event("round over");
            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundResult, 6f);
            trace.Event("round result");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Waiting, 6f);
            yield return EditorAppDriver.WaitUntilRendering(() => MofaTestActions.SummaryBoardText() != null,
                "The spectator did not see the summary board.", 3f);
            yield return EditorAppDriver.RenderMainCameraFor(0.5f);
            trace.Event("summary");
            yield return AgreeOnFinalState(manager);

            yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 30f);
            trace.Event("next round");

            trace.AssertNoErrors();
            trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-duel", $"{Scenario}-{MultiplayerScenario.Role}"));
            yield return MultiplayerScenario.Barrier("finished");
        }

        /// <summary>Every role publishes the final stats and summary board it sees; all three must be identical.</summary>
        private static IEnumerator AgreeOnFinalState(MofaBaseRealityManager manager)
        {
            var state = string.Join("; ", manager.MofaPlayerDict.Values.OrderBy(p => p.Team.Value)
                .Select(p => $"{p.Team.Value} {MofaTestActions.Stats(p)} distance {p.Distance.Value:0.00}"))
                + " || board: " + MofaTestActions.SummaryBoardText();
            MultiplayerScenario.Publish("final", state);
            Debug.Log($"[E2E] {Scenario} {MultiplayerScenario.Role} {state}");
            foreach (var role in new[] { MultiplayerScenario.Host, MultiplayerScenario.Player, MultiplayerScenario.Spectator })
            {
                yield return MultiplayerScenario.WaitFor(role, "final");
                Assert.That(MultiplayerScenario.Read(role, "final"), Is.EqualTo(state), $"The {role} sees a different result.");
            }
        }
    }
}
