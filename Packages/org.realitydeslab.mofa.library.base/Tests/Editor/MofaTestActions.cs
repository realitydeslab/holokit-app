using System;
using System.Collections;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using TMPro;
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
        /// Looks at a point and, once the player can cast (its life shield is intact: a broken shield blocks casting
        /// until it renovates) and a basic spell is charged, swings the Apple Watch to cast along the camera's view.
        /// Reports whether a spell was cast (false when the fight ended while waiting).
        /// </summary>
        public static IEnumerator CastAt(SimulatedPerson person, Func<Vector3> target, Action<bool> cast)
        {
            yield return person.LookAt(target());
            var player = Manager.LocalMofaPlayer;
            var before = player.CastCount.Value;
            // The input manager re-checks the shield in FixedUpdate, so wait until one has run since the player could cast.
            var castableSince = float.NaN;
            bool CanCast()
            {
                if (player.LifeShield == null || player.LifeShield.IsDestroyed || Input.BasicSpellChargePercentage < OneBasicCharge)
                {
                    castableSince = float.NaN;
                    return false;
                }
                if (float.IsNaN(castableSince)) castableSince = Time.fixedTime;
                return Time.fixedTime > castableSince;
            }
            IEnumerator WaitToCast() => EditorAppDriver.WaitUntilRendering(() => CanCast() || Phase != MofaPhase.Fighting,
                "The player could not cast: the shield stayed broken or the basic spell did not charge.", 8f);
            yield return WaitToCast();
            if (Phase != MofaPhase.Fighting) { cast(false); yield break; }
            yield return person.LookAt(target());   // the target may have moved while charging
            yield return WaitToCast();              // a spell may have broken the shield meanwhile
            if (Phase != MofaPhase.Fighting) { cast(false); yield break; }
            MofaWatchConnectivityAPI.EmitSimulatedWatchTriggerForDeviceTest();
            yield return EditorAppDriver.WaitUntilRendering(() => player.CastCount.Value > before || Phase != MofaPhase.Fighting,
                "A charged Watch swing did not cast a spell.", 2f);
            cast(player.CastCount.Value > before);
        }

        /// <summary>The summary board's text as a person reads it, or null while no board is shown.</summary>
        public static string SummaryBoardText()
        {
            var board = Object.FindAnyObjectByType<SummaryBoard>();
            return board == null ? null : string.Join(" | ", board.GetComponentsInChildren<TMP_Text>().Select(text => text.text));
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
