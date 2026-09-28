using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.TheHunting;
using RealityDesignLab.MOFA.TheHunting.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// MOFA Hunting with three Editors: the host controls the dragon, one player fights it, and a
    /// spectator watches. Run with <c>Tools/run_multiplayer_test.sh MOFA MofaHuntingMultiplayerTests</c>.
    /// Watch swings are simulated after recognition; the spec is in the package README.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class MofaHuntingMultiplayerTests
    {
        private const string Scene = "MOFATheHunting";

        [UnityTest, Timeout(600000)]
        public IEnumerator HostDragonFightsPlayerWhileSpectatorWatches()
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
                case MultiplayerScenario.Host: return HostRole();
                case MultiplayerScenario.Player: return PlayerRole();
                case MultiplayerScenario.Spectator: return SpectatorRole();
                default: throw new System.ArgumentException("Unknown HOLOKIT_MP_ROLE " + MultiplayerScenario.Role);
            }
        }

        private static IEnumerator HostRole()
        {
            yield return EditorAppDriver.EnterReality(Scene, true, HoloKitAppPlayerType.Player);
            MultiplayerScenario.Publish("hosting");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "joined");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, "joined");
            var app = HoloKitApp.Instance;
            var hunting = (MofaHuntingRealityManager)app.RealityManager;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.PlayerCount == 3, "Host did not see three peers.");

            // The spectator is connected but is not a combatant.
            // A joining peer counts as a player until its InitPlayerInfo RPC reports the spectator role.
            var spectatorId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Spectator, "joined"));
            var playerId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Player, "joined"));
            yield return EditorAppDriver.WaitUntil(() => !hunting.MofaPlayerDict.ContainsKey(spectatorId),
                "The spectator must not be a combatant.", 10f);
            Assert.That(hunting.MofaPlayerDict.ContainsKey(playerId), Is.True, "The player must be a combatant.");

            // Summon the dragon.
            var panel = Object.FindObjectsByType<MofaHuntingDragonControllerUIPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None).Single();
            panel.OnSpawnDragonButtonPressedFunc();
            yield return EditorAppDriver.WaitUntil(() => hunting.DragonController != null && hunting.CurrentPhase.Value == MofaPhase.Fighting,
                "The dragon did not spawn and start the fight.", 60f);
            MultiplayerScenario.Publish("dragon-spawned");

            // Lock onto the player and attack until the player's life shield takes damage.
            var target = hunting.MofaPlayerDict[playerId];
            hunting.LockTargetForTest(playerId);
            // Press the on-screen attack button the way a finger does; it raises the Malbers attack
            // events the dragon listens to (OnDragonPrimaryAttackButtonPressed has no subscribers).
            var attackButton = EditorAppDriver.FindVisibleUi("Primary Attack Button");
            var deadline = Time.realtimeSinceStartup + 45f;
            while (!ShieldDamaged(target) && Time.realtimeSinceStartup < deadline)
            {
                EditorAppDriver.Tap(attackButton);
                yield return new WaitForSecondsRealtime(1.5f);
            }
            Assert.That(ShieldDamaged(target), Is.True, "The dragon's attacks on the locked player did not damage its life shield.");
            MultiplayerScenario.Publish("dragon-hit-player");

            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "player-hit-dragon");
            yield return EditorAppDriver.WaitUntil(() => hunting.DragonController.CurrentHealthPercent < 1f,
                "The host did not see the dragon lose health.");
            yield return PublishFinalState(hunting);

            // Every role sees the same dragon health.
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "final");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Spectator, "final");
            var host = Health(MultiplayerScenario.Host);
            Assert.That(Health(MultiplayerScenario.Player), Is.EqualTo(host).Within(0.001f), "Player sees a different dragon health.");
            Assert.That(Health(MultiplayerScenario.Spectator), Is.EqualTo(host).Within(0.001f), "Spectator sees a different dragon health.");
        }

        private static IEnumerator PlayerRole()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Player);
            var app = HoloKitApp.Instance;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The player did not join the host.");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());

            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "dragon-spawned");
            var hunting = (MofaHuntingRealityManager)app.RealityManager;
            yield return EditorAppDriver.WaitUntil(() => hunting.DragonController != null && hunting.CurrentPhase.Value == MofaPhase.Fighting,
                "The player did not see the dragon and the fight start.");
            var local = hunting.LocalMofaPlayer;
            var input = Object.FindAnyObjectByType<MofaInputManager>();
            var casts = 0;
            var deadline = Time.realtimeSinceStartup + 60f;
            while (hunting.DragonController.CurrentHealthPercent >= 1f && Time.realtimeSinceStartup < deadline)
            {
                yield return EditorAppDriver.FaceMainCameraTowards(hunting.DragonController.transform.position);
                if (input.BasicSpellChargePercentage > 0.5f)
                {
                    MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                    casts++;
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                yield return null;
            }
            Assert.That(casts, Is.GreaterThan(0), "The player's basic spell never charged.");
            Assert.That(local.CastCount.Value, Is.GreaterThan(0), "Simulated Watch swings did not cast spells.");
            Assert.That(hunting.DragonController.CurrentHealthPercent, Is.LessThan(1f),
                $"{casts} spells did not damage the dragon.");
            MultiplayerScenario.Publish("player-hit-dragon", local.HitCount.Value.ToString());

            // The dragon's hit on this player replicates here.
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "dragon-hit-player");
            yield return EditorAppDriver.WaitUntil(() => ShieldDamaged(local), "The player did not see its own shield damaged.");
            yield return PublishFinalState(hunting);
        }

        private static IEnumerator SpectatorRole()
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(Scene, false, HoloKitAppPlayerType.Spectator);
            var app = HoloKitApp.Instance;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, "The spectator did not join the host.");
            MultiplayerScenario.Publish("joined", NetworkManager.Singleton.LocalClientId.ToString());

            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "dragon-spawned");
            var hunting = (MofaHuntingRealityManager)app.RealityManager;
            yield return EditorAppDriver.WaitUntil(() => hunting.DragonController != null, "The spectator does not see the dragon.");

            // A Watch swing from a spectator does not cast.
            var self = app.MultiplayerManager.LocalPlayer as MofaPlayer;
            for (var i = 0; i < 3; i++)
            {
                MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                yield return new WaitForSecondsRealtime(1f);
            }
            Assert.That(self == null || self.CastCount.Value == 0, Is.True, "A spectator must not be able to cast.");

            // The spectator sees both sides of the fight.
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Player, "player-hit-dragon");
            yield return EditorAppDriver.WaitUntil(() => hunting.DragonController.CurrentHealthPercent < 1f,
                "The spectator did not see the dragon lose health.");
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "dragon-hit-player");
            var playerId = ulong.Parse(MultiplayerScenario.Read(MultiplayerScenario.Player, "joined"));
            yield return EditorAppDriver.WaitUntil(() => ShieldDamaged(hunting.MofaPlayerDict[playerId]),
                "The spectator did not see the player's shield damaged.");
            yield return EditorAppDriver.FaceMainCameraTowards(hunting.DragonController.transform.position);
            var stats = EditorAppDriver.RenderMainCamera(Path.Combine(EditorAppDriver.OutputDirectory("MofaHunting"), "spectator.png"));
            Assert.That(stats.Magenta, Is.LessThan(0.002f), "The spectator view renders error-shader magenta.");
            yield return PublishFinalState(hunting);
        }

        private static IEnumerator PublishFinalState(MofaHuntingRealityManager hunting)
        {
            // Let the last hits replicate before comparing.
            yield return new WaitForSecondsRealtime(3f);
            MultiplayerScenario.Publish("final", hunting.DragonController.CurrentHealthPercent.ToString("R", CultureInfo.InvariantCulture));
        }

        private static float Health(string role) =>
            float.Parse(MultiplayerScenario.Read(role, "final"), CultureInfo.InvariantCulture);

        private static bool ShieldDamaged(MofaPlayer player)
        {
            // A missing shield means the round has not set one up yet, not that it was hit.
            var shield = player.LifeShield;
            return player.Death.Value > 0 || (shield != null && (shield.CenterDestroyed.Value || shield.TopDestroyed.Value ||
                   shield.LeftDestroyed.Value || shield.RightDestroyed.Value));
        }
    }
}
