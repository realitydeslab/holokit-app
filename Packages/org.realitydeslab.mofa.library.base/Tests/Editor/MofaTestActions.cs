using System;
using System.Collections;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoloKit.Tests
{
    /// <summary>What a MOFA player does and sees, for end-to-end journeys (Training, Duel, Hunting).</summary>
    public static class MofaTestActions
    {
        /// <summary>A basic spell needs one second of charge: 1 s of a 5 s charge bar.</summary>
        private const float OneBasicCharge = 0.2f;

        public static MofaBaseRealityManager Manager => HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
        public static MofaInputManager Input => Object.FindAnyObjectByType<MofaInputManager>();
        public static MofaPhase Phase => Manager.CurrentPhase.Value;

        public static IEnumerator WaitForPhase(MofaPhase phase, float timeout) =>
            EditorAppDriver.WaitUntilRendering(() => Phase == phase, $"MOFA did not reach {phase}.", timeout);

        /// <summary>Taps the on-screen Trigger button, as a player does to get ready or to cast.</summary>
        public static void TapTrigger() => EditorAppDriver.Tap(EditorAppDriver.FindVisibleUi("Trigger Button"));

        /// <summary>Where to aim to hit a player: the centre of its life shield.</summary>
        public static Vector3 ShieldCentre(MofaPlayer player) => player.LifeShield.transform.position;

        /// <summary>
        /// Looks at a point and, once a basic spell is charged, swings the Apple Watch to cast it along the
        /// camera's view. Reports whether a spell was cast (false when the fight ended while charging).
        /// </summary>
        public static IEnumerator CastAt(SimulatedPerson person, Func<Vector3> target, Action<bool> cast)
        {
            yield return person.LookAt(target());
            var player = Manager.LocalMofaPlayer;
            var before = player.CastCount.Value;
            yield return EditorAppDriver.WaitUntilRendering(() => Input.BasicSpellChargePercentage >= OneBasicCharge || Phase != MofaPhase.Fighting,
                "The basic spell did not charge.", 4f);
            if (Phase != MofaPhase.Fighting) { cast(false); yield break; }
            yield return person.LookAt(target());   // the target may have moved while charging
            MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
            yield return EditorAppDriver.WaitUntilRendering(() => player.CastCount.Value > before || Phase != MofaPhase.Fighting,
                "A charged Watch swing did not cast a spell.", 2f);
            cast(player.CastCount.Value > before);
        }

        public static string Stats(MofaPlayer player) => player == null ? "-"
            : $"kill {player.Kill.Value} death {player.Death.Value} cast {player.CastCount.Value} hit {player.HitCount.Value}";

        public static void AssertConsistentResult(MofaPlayer player, MofaPlayer opponent)
        {
            var stats = Manager.GetPlayerStats(player);
            var expected = player.Kill.Value > opponent.Kill.Value ? MofaPersonalRoundResult.Victory
                : player.Kill.Value < opponent.Kill.Value ? MofaPersonalRoundResult.Defeat : MofaPersonalRoundResult.Draw;
            Assert.That(stats.PersonalRoundResult, Is.EqualTo(expected),
                $"Round result {stats.PersonalRoundResult} does not follow the kills ({player.Kill.Value} vs {opponent.Kill.Value}).");
        }
    }
}
