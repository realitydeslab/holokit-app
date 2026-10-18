using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.TheHunting;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors, as the product is played: the host enters as the dragon (puppeteer), a player
    /// joins to slay it and a spectator watches, each standing in the test arena. The host summons the dragon,
    /// locks onto the player and attacks it with fireballs; the player casts at the dragon with Apple Watch swings
    /// until it dies and, as a person would, keeps swinging during the death animation. The round must end exactly
    /// once on every peer; then the host summons the next dragon. Each role's events are compared with
    /// Tests/Editor/Baselines/MofaHuntingJourney-&lt;role&gt;.json.
    /// Run with <c>Tools/run_multiplayer_test.sh MOFA MofaHuntingJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class MofaHuntingJourneyTests
    {
        private const string Scene = "MOFATheHunting";
        private const string Scenario = "MofaHuntingJourney";
        private const float AttackInterval = 2.5f;
        // The round ends after the 5 s death animation, then 3 s round over and 3 s result; a death triggered again
        // by a late hit would start within the next 5 s, so watch that long after the round ends.
        private const float SettleSeconds = 8f;

        private static readonly Vector3 PlayerSpot = new(1.2f, 0f, 0.5f);
        private static readonly Vector3 SpectatorSpot = new(-2.5f, 0f, 0.5f);

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator ThePlayerSlaysTheHostsDragonWhileASpectatorWatches()
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
                case MultiplayerScenario.Host: return Dragon();
                case MultiplayerScenario.Player: return Slayer();
                case MultiplayerScenario.Spectator: return Spectator();
                default: throw new ArgumentException("Unknown HOLOKIT_MP_ROLE " + MultiplayerScenario.Role);
            }
        }

        private static MofaHuntingRealityManager Hunting => (MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager;

        private static DragonController TheDragon => Hunting.DragonController;

        private static bool DragonAlive => TheDragon != null && TheDragon.CurrentHealthPercent > 0f;

        /// <summary>Where to aim at the dragon: the middle of its body as drawn.</summary>
        internal static Vector3 DragonBody()
        {
            var renderers = TheDragon.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers.Length == 0) return TheDragon.transform.position + Vector3.up;
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            return bounds.center;
        }

        /// <summary>What every role records: the phase, the round and the dragon's state. (Who counts as a slayer is
        /// asserted once everyone has joined: a joining spectator counts as a player until its role arrives.)</summary>
        private static E2ETrace StartTrace(List<MofaPhase> phases, Func<int> deaths)
        {
            var trace = new E2ETrace($"{Scenario}-{MultiplayerScenario.Role}");
            trace.Probe("phase", () => Hunting.CurrentPhase.Value);
            trace.Probe("round", () => Hunting.RoundCount.Value);
            trace.Probe("dragonAlive", () => DragonAlive);   // a dead dragon despawns 3 s after the round ends
            trace.Probe("dragonDeaths", () => deaths());
            trace.StartRecording();
            MofaBaseRealityManager.OnMofaPhaseChanged += phases.Add;
            return trace;
        }

        /// <summary>
        /// After the round: the dragon died once and the round ended once (round over, result, waiting), however many
        /// spells hit the dying dragon, and the next round has not been counted twice.
        /// </summary>
        private static IEnumerator AssertTheRoundEndedOnce(E2ETrace trace, List<MofaPhase> phases, Func<int> deaths)
        {
            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundOver, 30f);
            trace.Event("round over");
            yield return MofaTestActions.WaitForPhase(MofaPhase.RoundResult, 6f);
            trace.Event("round result");
            yield return MofaTestActions.WaitForPhase(MofaPhase.Waiting, 6f);
            trace.Event("waiting");
            yield return EditorAppDriver.RenderMainCameraFor(SettleSeconds);
            MofaBaseRealityManager.OnMofaPhaseChanged -= phases.Add;
            Assert.That(deaths(), Is.EqualTo(1), "The dragon died more than once: hits on the dying dragon killed it again.");
            Assert.That(phases.SkipWhile(p => p != MofaPhase.Fighting), Is.EqualTo(new[]
                { MofaPhase.Fighting, MofaPhase.RoundOver, MofaPhase.RoundResult, MofaPhase.Waiting }),
                "The round must end exactly once.");
            Assert.That(Hunting.RoundCount.Value, Is.EqualTo(2), "The next round was counted more than once.");
        }

        /// <summary>Every role publishes what it saw of the round; all three must be identical.</summary>
        private static IEnumerator AgreeOnFinalState(int deaths)
        {
            var state = $"round {Hunting.RoundCount.Value} dragon deaths {deaths}; " + string.Join("; ",
                Hunting.MofaPlayerDict.Values.OrderBy(p => p.OwnerClientId).Select(p => $"{p.Team.Value} {MofaTestActions.Stats(p)}"));
            MultiplayerScenario.Publish("final", state);
            Debug.Log($"[E2E] {Scenario} {MultiplayerScenario.Role} {state}");
            foreach (var role in new[] { MultiplayerScenario.Host, MultiplayerScenario.Player, MultiplayerScenario.Spectator })
            {
                yield return MultiplayerScenario.WaitFor(role, "final");
                Assert.That(MultiplayerScenario.Read(role, "final"), Is.EqualTo(state), $"The {role} saw a different round.");
            }
        }

        private static IEnumerator Dragon()
        {
            yield return EditorAppDriver.EnterReality(Scene, true, HoloKitAppPlayerType.Puppeteer);
            MultiplayerScenario.Publish("hosting");
            var person = SimulatedPerson.Spawn();
            var phases = new List<MofaPhase>();
            var deaths = 0;
            void OnDied() => deaths++;
            DragonController.OnDragonDied += OnDied;
            try
            {
                using var trace = StartTrace(phases, () => deaths);
                trace.Event("entered");
                MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());
                var placement = Object.FindAnyObjectByType<ARPlacementManager>();
                yield return EditorAppDriver.WaitUntilRendering(() => placement != null && placement.IsValid,
                    "The placement indicator for the dragon did not appear.", 5f);

                // The dragon controller is not a slayer, nor is the spectator.
                yield return MultiplayerScenario.Barrier("positioned");
                var playerId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Player, "joined"));
                yield return EditorAppDriver.WaitUntilRendering(() => Hunting.MofaPlayerDict.Keys.SequenceEqual(new[] { playerId }),
                    "Only the player must be a dragon slayer.", 10f);
                trace.Event("positioned");

                EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Spawn Dragon Button"));
                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 10f);
                trace.Event("countdown");
                yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
                Assert.That(TheDragon, Is.Not.Null, "Summoning did not spawn the dragon.");
                trace.Event("fighting");

                // The dragon flies out of its portal; the host gets the controls once it lands.
                yield return EditorAppDriver.WaitUntilRendering(() => EditorAppDriver.IsUiVisible("Floating Joystick"),
                    "The dragon never became controllable.", 20f);
                yield return person.LookAt(DragonBody());
                var view = default(EditorAppDriver.RenderStats);
                yield return trace.CapturePlayerView("the dragon", stats => view = stats);
                Assert.That(view.Magenta, Is.LessThan(0.002f), "The dragon renders error-shader magenta.");
                Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The host does not see the dragon.");
                trace.Event("dragon ready");

                // Lock onto the player: Lock shows an indicator over each slayer; picking one is a world-space tap,
                // which LockTargetForTest stands in for.
                EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Lock Button"));
                yield return EditorAppDriver.WaitUntilRendering(() => Object.FindObjectsByType<LockTargetPlayerIndicator>(FindObjectsSortMode.None).Length == 1,
                    "Lock did not show one indicator over the player.", 3f);
                Hunting.LockTargetForTest(playerId);
                trace.Event("locked the player");

                // Breathe fireballs at the player until one breaks its shield, then let the player fight.
                var player = Hunting.MofaPlayerDict[playerId];
                for (var t = 0f; player.Death.Value == 0 && t < 45f; t += AttackInterval)
                {
                    EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Primary Attack Button"));
                    yield return EditorAppDriver.RenderMainCameraFor(AttackInterval);
                }
                Assert.That(player.Death.Value, Is.GreaterThan(0), "The dragon's fireballs never broke the locked player's shield.");
                trace.Event("dragon hit the player");

                yield return EditorAppDriver.WaitUntilRendering(() => !DragonAlive, "The player did not slay the dragon.", 180f);
                trace.Event("dragon died");
                yield return AssertTheRoundEndedOnce(trace, phases, () => deaths);
                yield return AgreeOnFinalState(deaths);

                // The summon button comes back for the next dragon.
                EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Spawn Dragon Button"));
                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 10f);
                trace.Event("next round");
                trace.AssertNoErrors();
                trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-hunting", $"{Scenario}-{MultiplayerScenario.Role}"));
            }
            finally
            {
                DragonController.OnDragonDied -= OnDied;
            }
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator Slayer()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Player);
            var app = HoloKitApp.Instance;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The player did not join the host.");
            var person = SimulatedPerson.Spawn();
            var start = person.Phone.position;
            var phases = new List<MofaPhase>();
            var deaths = 0;
            void OnDied() => deaths++;
            DragonController.OnDragonDied += OnDied;
            try
            {
                using var trace = StartTrace(phases, () => deaths);
                trace.Event("entered");
                MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());
                yield return person.WalkTo(start + PlayerSpot);
                yield return person.LookAt(start + PlayerSpot + Vector3.forward * 3f);
                yield return MultiplayerScenario.Barrier("positioned");
                yield return EditorAppDriver.WaitUntilRendering(() => Hunting.MofaPlayerDict.Count == 1,
                    "Only the player must be a dragon slayer.", 10f);
                trace.Event("positioned");

                // The host's summon gets every slayer ready.
                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 60f);
                trace.Event("countdown");
                yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
                yield return EditorAppDriver.WaitUntilRendering(() => TheDragon != null, "The player does not see the dragon.", 5f);
                trace.Event("fighting");
                yield return person.LookAt(DragonBody());
                var view = default(EditorAppDriver.RenderStats);
                yield return trace.CapturePlayerView("the dragon", stats => view = stats);
                Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The player does not see the dragon.");

                // Cast at the dragon until it dies, then keep swinging, as a person does, until the round is over.
                var me = Hunting.LocalMofaPlayer;
                var casts = 0;
                var castsAtDying = 0;
                while (MofaTestActions.Phase == MofaPhase.Fighting)
                {
                    if (castsAtDying == 0 && !DragonAlive) trace.Event("dragon died");
                    if (!DragonAlive) castsAtDying++;
                    var cast = false;
                    yield return MofaTestActions.CastAt(person, () => TheDragon != null ? DragonBody() : person.Phone.position + person.Phone.forward,
                        result => cast = result);
                    if (cast) casts++;
                }
                Assert.That(castsAtDying, Is.GreaterThan(0), "The player stopped swinging before the dragon's death animation.");
                Debug.Log($"[E2E] {Scenario} player swung {castsAtDying} times at the dying dragon; {casts} spells in all");
                Assert.That(me.CastCount.Value, Is.EqualTo(casts), "The player's cast count is not the number of spells cast.");
                Assert.That(me.Death.Value, Is.GreaterThan(0), "The player did not see the dragon break its shield.");
                yield return AssertTheRoundEndedOnce(trace, phases, () => deaths);
                yield return AgreeOnFinalState(deaths);

                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 60f);
                trace.Event("next round");
                trace.AssertNoErrors();
                trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-hunting", $"{Scenario}-{MultiplayerScenario.Role}"));
            }
            finally
            {
                DragonController.OnDragonDied -= OnDied;
            }
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator Spectator()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Spectator);
            var app = HoloKitApp.Instance;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The spectator did not join the host.");
            var person = SimulatedPerson.Spawn();
            var start = person.Phone.position;
            var phases = new List<MofaPhase>();
            var deaths = 0;
            void OnDied() => deaths++;
            DragonController.OnDragonDied += OnDied;
            try
            {
                using var trace = StartTrace(phases, () => deaths);
                trace.Event("entered");
                MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());
                yield return person.WalkTo(start + SpectatorSpot);
                yield return person.LookAt(start + Vector3.forward * 4f);
                yield return MultiplayerScenario.Barrier("positioned");
                yield return EditorAppDriver.WaitUntilRendering(() => Hunting.MofaPlayerDict.Count == 1,
                    "The spectator must not count as a dragon slayer.", 10f);
                trace.Event("positioned");

                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 60f);
                trace.Event("countdown");
                yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 6f);
                yield return EditorAppDriver.WaitUntilRendering(() => TheDragon != null, "The spectator does not see the dragon.", 5f);
                trace.Event("fighting");

                // A spectator's Watch swings cast nothing.
                var self = app.MultiplayerManager.LocalPlayer as MofaPlayer;
                for (var i = 0; i < 3; i++)
                {
                    MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                    yield return EditorAppDriver.RenderMainCameraFor(1f);
                }
                Assert.That(self == null || self.CastCount.Value == 0, Is.True, "A spectator must not be able to cast.");
                yield return person.LookAt(DragonBody());
                var view = default(EditorAppDriver.RenderStats);
                yield return trace.CapturePlayerView("the dragon from the side", stats => view = stats);
                Assert.That(view.Magenta, Is.LessThan(0.002f), "The spectator view renders error-shader magenta.");
                Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The spectator does not see the dragon.");

                yield return EditorAppDriver.WaitUntilRendering(() => !DragonAlive, "The spectator did not see the dragon die.", 240f);
                trace.Event("dragon died");
                yield return AssertTheRoundEndedOnce(trace, phases, () => deaths);
                yield return AgreeOnFinalState(deaths);

                yield return MofaTestActions.WaitForPhase(MofaPhase.Countdown, 60f);
                trace.Event("next round");
                trace.AssertNoErrors();
                trace.AssertMatchesBaseline(E2ETrace.BaselinePath("org.realitydeslab.mofa.the-hunting", $"{Scenario}-{MultiplayerScenario.Role}"));
            }
            finally
            {
                DragonController.OnDragonDied -= OnDied;
            }
            yield return MultiplayerScenario.Barrier("finished");
        }
    }
}
