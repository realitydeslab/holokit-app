using System.Collections;
using System.Collections.Generic;
using Holoi.Library.ARUX;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// The Apple Watch's ultimate in MOFA The Training: holding the Ground pose charges it and a swing casts it. Each
    /// round allows the spell's MaxUseCount ultimates, and the next round allows them again.
    /// </summary>
    public sealed class MofaTrainingWatchTests
    {
        // Long enough for MysticArt's 10 ultimates (3 s charge each) and the one refused after them.
        private const float RoundSeconds = 60f;

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator EveryRoundAllowsTheFullUltimateCount()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(Rounds());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Rounds()
        {
            yield return EditorAppDriver.EnterReality("MOFATheTraining", true, HoloKitAppPlayerType.Player);
            var manager = MofaTestActions.Manager;
            var placement = Object.FindAnyObjectByType<ARPlacementManager>();
            yield return EditorAppDriver.WaitUntilRendering(() => placement != null && placement.IsValid,
                "The placement indicator for the AI did not appear.", 5f);
            manager.RoundDuration = RoundSeconds;
            var allowed = MofaTestActions.Input.SecondarySpellMaxUseCount;
            var casts = new List<int>();
            for (var round = 0; round < 2; round++)
            {
                MofaTestActions.TapTrigger();
                yield return MofaTestActions.WaitForPhase(MofaPhase.Fighting, 10f);
                var count = 0;
                yield return CastUltimatesUntilRefused(allowed + 1, n => count = n);
                casts.Add(count);
                MofaWatchConnectivityAPI.EmitSimulatedWatchStateForDeviceTest(MofaWatchState.Normal);
                yield return MofaTestActions.WaitForPhase(MofaPhase.Waiting,
                    RoundSeconds + manager.RoundOverDuration + manager.RoundResultDuration + 10f);
            }
            Assert.That(casts, Is.EqualTo(new[] { allowed, allowed }),
                $"Ultimates cast per round (allowed {allowed}): {string.Join(", ", casts)}.");
        }

        /// <summary>
        /// Holds the Ground pose and swings whenever the ultimate is charged, until a charged swing casts nothing or
        /// <paramref name="attempts"/> swings were made. Reports how many ultimates were cast.
        /// </summary>
        private static IEnumerator CastUltimatesUntilRefused(int attempts, System.Action<int> report)
        {
            var player = MofaTestActions.Manager.LocalMofaPlayer;
            var input = MofaTestActions.Input;
            MofaWatchConnectivityAPI.EmitSimulatedWatchStateForDeviceTest(MofaWatchState.Ground);
            var cast = 0;
            // A charged ultimate takes every swing (no basic spell), so a cast while it is charged is the ultimate.
            var castableSince = float.NaN;
            bool Charged()
            {
                if (player.LifeShield == null || player.LifeShield.IsDestroyed || input.SecondarySpellChargePercentage < 1f)
                {
                    castableSince = float.NaN;
                    return false;
                }
                if (float.IsNaN(castableSince)) castableSince = Time.fixedTime;
                return Time.fixedTime > castableSince; // MofaInputManager re-checks the shield in FixedUpdate
            }
            for (var swing = 0; swing < attempts; swing++)
            {
                yield return EditorAppDriver.WaitUntilRendering(() => Charged(),
                    $"The ultimate did not charge in the Ground pose (after {cast} casts).", 15f);
                var before = player.CastCount.Value;
                MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
                yield return EditorAppDriver.RenderMainCameraFor(0.3f);
                if (player.CastCount.Value == before) break;
                cast++;
            }
            report(cast);
        }
    }
}
