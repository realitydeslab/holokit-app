using System.Collections;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// The simulated person carries the phone through XR Simulation: walking and turning move the
    /// tracked device, so the app sees the same camera motion a real player produces.
    /// </summary>
    public sealed class SimulatedPersonTests
    {
        // Hand tests need a scene with hand tracking (the HoloKit app's first scene, MOFA Training, has none).
        private static HoloKit.HoloKitHandTracker RequireHandTracking()
        {
            var tracker = HoloKit.HoloKitHandTracker.Instance;
            if (tracker == null) Assert.Ignore("This product's first scene has no hand tracker.");
            return tracker;
        }

        [TearDown] public void RestoreEnvironment() => SimulationArena.Restore();

        [UnityTest, Timeout(300000)]
        public IEnumerator TheTestArenaHasRoomToWalkFourMetresAndBack()
        {
            SimulationArena.Use();
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return WalkFar();
            yield return new ExitPlayMode();
        }

        private static IEnumerator WalkFar()
        {
            yield return EditorAppDriver.EnterFirstReality();
            var person = SimulatedPerson.Spawn();
            var start = person.Phone.position;
            // Session space has the simulated floor at 0; the XR Origin may add a camera offset in world space.
            Assert.That(person.Phone.localPosition.y, Is.InRange(1.3f, 1.6f), "The arena starts the phone at a person's chest height.");
            var ahead = Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up).normalized;
            yield return person.WalkTo(start + ahead * 4f);
            yield return person.WalkTo(start - ahead * 4f);
            yield return person.WalkTo(start);
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator WalkingMovesThePhoneThroughTrackingNotTheOrigin()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Walk();
            yield return new ExitPlayMode();
        }

        // Runs after the Play Mode domain reload; see EditorAppDriver.
        private static IEnumerator Walk()
        {
            yield return EditorAppDriver.EnterFirstReality();
            var person = SimulatedPerson.Spawn();
            var origin = Object.FindAnyObjectByType<XROrigin>();
            var originBefore = origin.transform.position;
            var start = person.Phone.position;
            var ahead = Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up).normalized;

            yield return person.WalkTo(start + ahead);
            Assert.That(Vector3.Distance(person.Phone.position, start + ahead), Is.LessThan(0.05f),
                "Walking 1 m ahead did not carry the phone there.");
            Assert.That(Vector3.Distance(origin.transform.position, originBefore), Is.LessThan(0.001f),
                "Walking must move the tracked phone, not the XR Origin.");

            var target = person.Phone.position + Vector3.Cross(Vector3.up, ahead) * 2f + Vector3.down * 0.5f;
            yield return person.LookAt(target);
            Assert.That(Vector3.Angle(person.Phone.forward, target - person.Phone.position), Is.LessThan(3f),
                "Turning the head did not point the phone at the target.");
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator ARaisedHandIsTrackedMovesWithThePersonAndStaysWithinReach()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Reach();
            yield return new ExitPlayMode();
        }

        private static IEnumerator Reach()
        {
            yield return EditorAppDriver.EnterFirstReality();
            var person = SimulatedPerson.Spawn();
            var tracker = RequireHandTracking();
            Vector3 Palm() => tracker.GetHandJointPosition(HoloKit.HandJoint.MiddleMCP);

            // A real hand is tracked only within arm's length and in front of the camera.
            Assert.That(person.CanReach(person.Phone.position + person.Phone.forward * 0.5f), Is.True);
            Assert.That(person.CanReach(person.Phone.position + person.Phone.forward * 1.5f), Is.False, "Beyond arm's length.");
            Assert.That(person.CanReach(person.Phone.position - person.Phone.forward * 0.5f), Is.False, "Behind the camera.");

            var target = person.Phone.position + person.Phone.forward * 0.5f;
            yield return person.RaiseHandTo(target);
            Assert.That(Vector3.Distance(Palm(), target), Is.LessThan(0.02f), "The palm is not where the hand reached.");
            Assert.That(Vector3.Distance(tracker.GetHandJointPosition(HoloKit.HandJoint.Wrist),
                tracker.GetHandJointPosition(HoloKit.HandJoint.MiddleTip)), Is.GreaterThan(0.1f), "The hand has no shape.");

            // The arm moves with the body.
            var ahead = Vector3.ProjectOnPlane(person.Phone.forward, Vector3.up).normalized * 0.5f;
            yield return person.WalkTo(person.Phone.position + ahead);
            Assert.That(Vector3.Distance(Palm(), target + ahead), Is.LessThan(0.05f), "The raised hand did not move with the person.");

            // Down by the person's side the hand is out of the camera's view, so the tracker loses it, as on a phone.
            yield return person.LowerHand();
            yield return EditorAppDriver.WaitUntilRendering(() => !tracker.IsValid, "The lowered hand is still tracked.", 2f);
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator AHandHeldOnAMovingTargetFollowsItUntilDone()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Follow();
            yield return new ExitPlayMode();
        }

        private static IEnumerator Follow()
        {
            yield return EditorAppDriver.EnterFirstReality();
            var person = SimulatedPerson.Spawn();
            var tracker = RequireHandTracking();
            var start = person.Phone.position + person.Phone.forward * 0.5f;
            var side = Vector3.Cross(Vector3.up, person.Phone.forward).normalized;
            var began = Time.realtimeSinceStartup;
            Vector3 Target() => start + side * Mathf.Min(0.2f, (Time.realtimeSinceStartup - began) * 0.1f);
            var maxLag = 0f;
            yield return person.HoldHandOn(Target, () =>
            {
                maxLag = Mathf.Max(maxLag, Vector3.Distance(tracker.GetHandJointPosition(HoloKit.HandJoint.MiddleMCP), Target()));
                return Time.realtimeSinceStartup - began > 2.5f;
            }, 5f, "Holding the hand did not finish.");
            Assert.That(maxLag, Is.LessThan(0.03f), "The palm did not stay on the moving target.");
            Assert.That(Vector3.Distance(tracker.GetHandJointPosition(HoloKit.HandJoint.MiddleMCP), start + side * 0.2f), Is.LessThan(0.02f));
        }

        [UnityTest, Timeout(300000)]
        public IEnumerator WithAnExtendedPointerThePalmStopsShortOfTheTargetOnTheViewRay()
        {
            EditorAppDriver.OpenStartScene();
            yield return new EnterPlayMode();
            yield return Extended();
            yield return new ExitPlayMode();
        }

        // Some games touch with a point projected beyond the hand along the eye-to-hand ray (ExtendedHandJoint).
        private static IEnumerator Extended()
        {
            yield return EditorAppDriver.EnterFirstReality();
            var person = SimulatedPerson.Spawn();
            var tracker = RequireHandTracking();
            var target = person.Phone.position + person.Phone.forward * 1.1f;
            var held = false;
            yield return person.HoldHandOn(() => target, () => held = true, 1f, "Holding did not run.", extension: 0.5f);
            var palm = tracker.GetHandJointPosition(HoloKit.HandJoint.MiddleMCP);
            Assert.That(Vector3.Distance(palm, person.Phone.position + person.Phone.forward * 0.6f), Is.LessThan(0.02f),
                "With a 0.5 m extension the palm should be 0.5 m short of a target 1.1 m away, on the view ray.");
            Assert.That(held, Is.True);
        }
    }
}
