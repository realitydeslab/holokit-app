using System.Collections;
using System.Linq;
using Holoi.Reality.Typography;
using NUnit.Framework;
using RealityDesignLab.Typed.TheScanner;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.ARFoundation;

namespace HoloKit.Tests
{
    /// <summary>
    /// End to end with three Editors (<see cref="SpectatedJourney"/>): as the host walks around, the room is meshed
    /// and every mesh draws the scanner's ribbons centred on the scanner centre; the waiting hint fades. Spectators,
    /// one watching from the start and one joining later, mesh their own view and centre the ribbons on the host's
    /// scanner centre.
    /// Run with <c>Tools/run_multiplayer_test.sh Typed TypedTheScannerJourneyTests</c>.
    /// </summary>
    [Explicit("Needs three Editors; run with Tools/run_multiplayer_test.sh.")]
    [Category("Multiplayer")]
    public sealed class TypedTheScannerJourneyTests
    {
        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(900000)]
        public IEnumerator TheRoomIsScannedAroundTheScannerCentre()
        {
            MultiplayerScenario.RequireConfigured();
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return MultiplayerScenario.Guard(EditorAppDriver.Run(SpectatedJourney.Run("TypedTheScanner", "TypedTheScannerJourney",
                "org.realitydeslab.typed.the-scanner", null, Host, Spectator)));
            yield return new ExitPlayMode();
        }

        private static Vector3 Centre => TheScannerCenter.Instance.transform.position;
        private static int Meshes => Object.FindAnyObjectByType<ARMeshManager>()?.meshes.Count ?? 0;

        private static bool HintShown => Object.FindAnyObjectByType<TypedRealityTheScannerUIPanel>() is { } panel &&
                                         panel.GetComponentsInChildren<TMP_Text>().Any(text => text.gameObject.activeInHierarchy);

        private static IEnumerator Host(SpectatedJourney journey)
        {
            var person = journey.Person;
            yield return EditorAppDriver.WaitUntilRendering(() => Meshes > 0, "The room was not meshed.", 15f);
            yield return person.WalkTo(journey.Start + new Vector3(0f, 0f, -2f));
            yield return person.LookAt(person.Phone.position + new Vector3(0f, -1.2f, 1.5f));
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            AssertMeshesCentredOnTheScanner();
            var view = default(EditorAppDriver.RenderStats);
            yield return journey.Trace.CapturePlayerView("the scanned floor", stats => view = stats);
            Assert.That(view.Magenta, Is.LessThan(0.002f), "The scanned room renders error-shader magenta.");
            Assert.That(view.LitCenter, Is.GreaterThan(0.02f), "The scanner draws nothing on the floor.");
            journey.Trace.Event("room scanned");
            yield return EditorAppDriver.WaitUntilRendering(() => !HintShown, "The waiting hint did not fade.", 10f);
            journey.Trace.Event("hint faded");
            yield return journey.Share("centre", Centre, final: true);
        }

        private static void AssertMeshesCentredOnTheScanner()
        {
            foreach (var mesh in Object.FindObjectsByType<TheScannerMeshController>(FindObjectsSortMode.None))
                Assert.That(Vector3.Distance(mesh.GetComponent<MeshRenderer>().material.GetVector("_Center_Position"), Centre), Is.LessThan(0.01f),
                    $"Mesh {mesh.name} draws its ribbons around another centre.");
        }

        private static IEnumerator Spectator(SpectatedJourney journey)
        {
            yield return journey.Expect("centre", () => Centre, 0.01f);
            yield return EditorAppDriver.WaitUntilRendering(() => Meshes > 0, "The spectator's view was not meshed.", 15f);
            yield return EditorAppDriver.RenderMainCameraFor(1f);
            AssertMeshesCentredOnTheScanner();
            yield return EditorAppDriver.WaitUntilRendering(() => !HintShown, "The spectator's waiting hint did not fade.", 10f);
            journey.Trace.Event("sees the scanned room");
        }
    }
}
