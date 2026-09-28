using System.Collections;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.TheDuel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// MOFA Duel with three Editors: two players fight each other and a spectator watches.
    /// Run with <c>Tools/run_multiplayer_test.sh MOFA MofaDuelMultiplayerTests</c>.
    /// Watch swings are simulated after recognition; the spec is in the package README.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class MofaDuelMultiplayerTests
    {
        private const string Scene = "MOFATheDuel";

        // Editor peers share one simulated room; the second player stands this far away, facing back.
        private static readonly Vector3 PlayerOffset = new(0f, 0f, 3f);

        [UnityTest, Timeout(600000)]
        public IEnumerator TwoPlayersDuelWhileSpectatorWatches()
        {
            MultiplayerScenario.RequireConfigured();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(RunRole());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator RunRole()
        {
            switch (MultiplayerScenario.Role)
            {
                case MultiplayerScenario.Host: return FighterRole(true);
                case MultiplayerScenario.Player: return FighterRole(false);
                case MultiplayerScenario.Spectator: return SpectatorRole();
                default: throw new System.ArgumentException("Unknown HOLOKIT_MP_ROLE " + MultiplayerScenario.Role);
            }
        }

        private static IEnumerator FighterRole(bool isHost)
        {
            if (!isHost) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, isHost, HoloKitAppPlayerType.Player);
            var app = HoloKitApp.Instance;
            var duel = (MofaDuelRealityManager)app.RealityManager;
            if (isHost) MultiplayerScenario.Publish("hosting");
            yield return EditorAppDriver.WaitUntil(() => duel.LocalMofaPlayer != null, "The local MOFA player did not spawn.");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());
            if (isHost)
            {
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "joined");
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, "joined");
                yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.PlayerCount == 3, "Host did not see three peers.");
                // A joining peer counts as a player until its InitPlayerInfo RPC reports the spectator role.
                var spectatorId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Spectator, "joined"));
                yield return EditorAppDriver.WaitUntil(() => !duel.MofaPlayerDict.ContainsKey(spectatorId),
                    "The spectator must not be a duelist.", 10f);
                Assert.That(duel.MofaPlayerDict.Count, Is.EqualTo(2), "The duel needs exactly two players.");
            }
            else
            {
                yield return EditorAppDriver.MovePlayer(PlayerOffset, 180f);
            }
            MultiplayerScenario.Publish("positioned");
            yield return MultiplayerScenario.WaitFor(isHost ? MultiplayerScenario.Player : MultiplayerScenario.Host, "positioned");

            // Each player taps the on-screen Trigger button to get ready; the round starts when both are.
            EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Trigger Button"));
            yield return EditorAppDriver.WaitUntil(() => duel.CurrentPhase.Value == MofaPhase.Fighting,
                "The duel round did not start after both players pressed Trigger.", 60f);

            // Players take turns: spells of opposing teams cancel each other mid-air, and two players
            // firing along the same line at the same rhythm would only ever hit each other's spells.
            var local = duel.LocalMofaPlayer;
            var opponent = duel.MofaPlayerDict.Values.Single(p => p != local);
            if (!isHost)
            {
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hit-opponent", 120f);
                yield return EditorAppDriver.WaitUntil(() => ShieldDamaged(local), "This player did not see its own shield damaged.");
            }

            // Cast at the opponent with simulated Watch swings until its shield is damaged.
            var input = Object.FindAnyObjectByType<MofaInputManager>();
            var deadline = Time.realtimeSinceStartup + 60f;
            while (!ShieldDamaged(opponent) && Time.realtimeSinceStartup < deadline)
            {
                if (opponent.LifeShield != null)
                    yield return EditorAppDriver.AimMainCameraAt(opponent.LifeShield.transform.position);
                if (input.BasicSpellChargePercentage > 0.5f)
                {
                    MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                yield return null;
            }
            Assert.That(local.CastCount.Value, Is.GreaterThan(0), "Simulated Watch swings did not cast spells.");
            Assert.That(ShieldDamaged(opponent), Is.True,
                $"{local.CastCount.Value} spells (hits {local.HitCount.Value}) did not damage the opponent's shield. " +
                $"phase={duel.CurrentPhase.Value} me={Camera.main.transform.position} facing={Camera.main.transform.forward} " +
                $"opponent shield={(opponent.LifeShield != null ? opponent.LifeShield.transform.position.ToString() : "none")} " +
                $"teams me={local.Team.Value} opponent={opponent.Team.Value}");
            MultiplayerScenario.Publish("hit-opponent");

            // The opponent's hit on this player replicates here.
            if (isHost)
            {
                yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "hit-opponent", 120f);
                yield return EditorAppDriver.WaitUntil(() => ShieldDamaged(local), "This player did not see its own shield damaged.");
            }
            yield return PublishFinalState(duel);
            if (isHost) yield return CompareFinalStates();
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator SpectatorRole()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Spectator);
            var app = HoloKitApp.Instance;
            var duel = (MofaDuelRealityManager)app.RealityManager;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The spectator did not join the host.");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());
            // Spectators get no Trigger button and a Watch swing casts nothing.
            Assert.That(Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Any(rect => rect.name == "Trigger Button"), Is.False, "A spectator must not see the Trigger button.");
            yield return EditorAppDriver.WaitUntil(() => duel.CurrentPhase.Value == MofaPhase.Fighting,
                "The spectator did not see the duel start.", 90f);
            var self = app.MultiplayerManager.LocalPlayer as MofaPlayer;
            for (var i = 0; i < 3; i++)
            {
                MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                yield return new WaitForSecondsRealtime(1f);
            }
            Assert.That(self == null || self.CastCount.Value == 0, Is.True, "A spectator must not be able to cast.");

            // The spectator sees both players' hits.
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hit-opponent", 180f);
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "hit-opponent", 180f);
            yield return EditorAppDriver.WaitUntil(() => duel.MofaPlayerDict.Values.All(ShieldDamaged),
                "The spectator did not see both shields damaged.");
            var fighter = duel.MofaPlayerDict.Values.First();
            if (fighter.LifeShield != null)
                yield return EditorAppDriver.FaceMainCameraTowards(fighter.LifeShield.transform.position);
            var stats = EditorAppDriver.RenderMainCamera(Path.Combine(EditorAppDriver.OutputDirectory("MofaDuel"), "spectator.png"));
            Assert.That(stats.Magenta, Is.LessThan(0.002f), "The spectator view renders error-shader magenta.");
            yield return PublishFinalState(duel);
            yield return MultiplayerScenario.Barrier("finished");
        }

        private static IEnumerator PublishFinalState(MofaDuelRealityManager duel)
        {
            // Let the last hits replicate before comparing.
            yield return new WaitForSecondsRealtime(3f);
            MultiplayerScenario.Publish("final", string.Join(";", duel.MofaPlayerDict.OrderBy(p => p.Key)
                .Select(p => $"{p.Key}:casts={p.Value.CastCount.Value},damaged={ShieldDamaged(p.Value)}")));
        }

        private static IEnumerator CompareFinalStates()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "final");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, "final");
            var host = MultiplayerScenario.Read(MultiplayerScenario.Host, "final");
            Assert.That(MultiplayerScenario.Read(MultiplayerScenario.Player, "final"), Is.EqualTo(host), "Player sees a different duel state.");
            Assert.That(MultiplayerScenario.Read(MultiplayerScenario.Spectator, "final"), Is.EqualTo(host), "Spectator sees a different duel state.");
        }

        private static bool ShieldDamaged(MofaPlayer player)
        {
            // A missing shield means the round has not set one up yet, not that it was hit.
            var shield = player.LifeShield;
            return player.Death.Value > 0 || (shield != null && (shield.CenterDestroyed.Value || shield.TopDestroyed.Value ||
                   shield.LeftDestroyed.Value || shield.RightDestroyed.Value));
        }
    }
}
