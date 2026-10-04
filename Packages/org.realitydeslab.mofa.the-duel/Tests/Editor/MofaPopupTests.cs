using System.Collections;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// The popups a MOFA player sees between rounds (shared by Training, Duel, Ghost and Hunting; run here in Duel).
    /// </summary>
    public sealed class MofaPopupTests
    {
        [UnityTest, Timeout(300000)]
        public IEnumerator TheSummaryBoardStaysAfterTheResultPopupTimesOut()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return EditorAppDriver.Run(SummaryBoardStays());
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator SummaryBoardStays()
        {
            yield return EditorAppDriver.EnterReality("MOFATheDuel", true, HoloKitAppPlayerType.Player);
            var manager = MofaTestActions.Manager;

            // The result popup (Victory, Defeat or Draw) times out after 3 s; the summary board replaces it when the
            // round goes back to Waiting, here 1 s later, and must stay until the next round.
            manager.CurrentPhase.Value = MofaPhase.RoundResult;
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            manager.CurrentPhase.Value = MofaPhase.Waiting;
            yield return EditorAppDriver.WaitUntilRendering(() => MofaTestActions.SummaryBoardText() != null,
                "The summary board did not appear in Waiting.", 2f);
            yield return EditorAppDriver.RenderMainCameraFor(3f);
            Assert.That(MofaTestActions.SummaryBoardText(), Is.Not.Null,
                "The summary board disappeared when the earlier result popup timed out.");
        }
    }
}
