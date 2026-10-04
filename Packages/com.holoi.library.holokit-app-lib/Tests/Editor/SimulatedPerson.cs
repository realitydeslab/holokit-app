using System.Collections;
using HoloKit;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Simulation;

namespace HoloKit.Tests
{
    /// <summary>
    /// A person carrying the phone in XR Simulation. Walking and turning move the simulated device
    /// (<see cref="SimulationCameraPoseProvider"/>), so tracking, plane discovery and raycasts follow as
    /// they do for a real player. Editor-only assemblies cannot add MonoBehaviours, so this is a plain object.
    /// </summary>
    public sealed class SimulatedPerson
    {
        public const float WalkingSpeed = 1.2f;   // metres per second
        public const float TurnSpeed = 120f;      // degrees per second
        private const float ArrivalTimeout = 2f;  // tracking lags the device by a frame or two
        public const float ArmReach = 0.75f;      // metres from the phone
        public const float CameraHalfAngle = 45f; // a hand is tracked only when the camera sees it

        // Where the hand is, in the body frame (phone position, heading only): an arm moves with the body,
        // not with the head's pitch. Null until the person first raises a hand.
        private Vector3? _hand;

        public static SimulatedPerson Spawn() => new();

        /// <summary>The phone's camera in world space.</summary>
        public Transform Phone => Camera.main.transform;

        // Tracked poses are local to the camera's parent (the XR Origin's camera offset).
        private Transform Session => Phone.parent;

        private static Transform Device
        {
            get
            {
                var device = Object.FindAnyObjectByType<SimulationCameraPoseProvider>(FindObjectsInactive.Include);
                Assert.That(device, Is.Not.Null, "XR Simulation is not running: there is no simulated device.");
                return device.transform;
            }
        }

        /// <summary>Walks to a point on the level, carrying the phone at its current height.</summary>
        public IEnumerator WalkTo(Vector3 worldPosition, float speed = WalkingSpeed)
        {
            var device = Device;
            var target = new Vector3(worldPosition.x, Phone.position.y, worldPosition.z);
            // Track progress here: XR Simulation clamps the device to the room's movement bounds.
            var current = device.position;
            var goal = current + Session.InverseTransformVector(target - Phone.position);
            while (current != goal)
            {
                current = Vector3.MoveTowards(current, goal, speed * Time.unscaledDeltaTime);
                device.position = current;
                yield return null;
            }
            yield return WaitForPhone(() => Vector3.Distance(Phone.position, target) < 0.01f,
                $"The phone did not arrive at {target}; it is at {Phone.position}. " +
                "XR Simulation keeps the phone inside the simulated room's movement bounds.");
        }

        /// <summary>Turns the head so the phone looks at a point (XR Simulation limits pitch to 85 degrees).</summary>
        public IEnumerator LookAt(Vector3 worldPosition, float speed = TurnSpeed)
        {
            var device = Device;
            var direction = worldPosition - Phone.position;
            var current = device.rotation;
            var goal = Quaternion.Inverse(Session.rotation) * Quaternion.LookRotation(direction, Vector3.up);
            while (current != goal)
            {
                current = Quaternion.RotateTowards(current, goal, speed * Time.unscaledDeltaTime);
                if (Quaternion.Angle(current, goal) < 0.01f) current = goal;
                device.rotation = current;
                yield return null;
            }
            yield return WaitForPhone(() => Vector3.Angle(Phone.forward, worldPosition - Phone.position) < 1f,
                $"The phone did not turn to look at {worldPosition}.");
        }

        /// <summary>Whether a hand can be tracked there: within arm's length and in front of the camera.</summary>
        public bool CanReach(Vector3 worldPosition)
        {
            var offset = worldPosition - Phone.position;
            return offset.magnitude <= ArmReach && Vector3.Angle(Phone.forward, offset) <= CameraHalfAngle;
        }

        /// <summary>Moves the open hand (palm) to a point in front of the camera and holds it there.</summary>
        public IEnumerator RaiseHandTo(Vector3 worldPosition, float seconds = 0.4f)
        {
            var offset = worldPosition - Phone.position;
            Assert.That(CanReach(worldPosition), Is.True,
                $"A hand cannot be tracked at {worldPosition}: {offset.magnitude:F2} m away (reach {ArmReach} m), " +
                $"{Vector3.Angle(Phone.forward, offset):F0} deg off the camera axis (limit {CameraHalfAngle}).");
            yield return MoveHand(() => Quaternion.Inverse(BodyRotation) * (worldPosition - Phone.position), seconds);
        }

        /// <summary>
        /// Keeps the palm on a target that may move (e.g. a button floating in front of the player) until
        /// <paramref name="until"/> holds; fails if the target leaves arm's reach or the camera's view.
        /// </summary>
        public IEnumerator HoldHandOn(System.Func<Vector3> target, System.Func<bool> until, float timeout, string failure)
        {
            Assert.That(CanReach(target()), Is.True, $"{failure} The target at {target()} is out of reach.");
            // Reach for where the target is now, frame by frame, not where it was when the reach began.
            yield return MoveHand(() => Quaternion.Inverse(BodyRotation) * (target() - Phone.position), 0.4f);
            for (var t = 0f; !until() && t < timeout; t += Time.unscaledDeltaTime)
            {
                var point = target();
                Assert.That(CanReach(point), Is.True, $"{failure} The target moved out of reach to {point}.");
                _hand = Quaternion.Inverse(BodyRotation) * (point - Phone.position);
                yield return null;
            }
            Assert.That(until(), Is.True, failure);
        }

        /// <summary>Lowers the hand to the person's side, out of view and away from anything in front.</summary>
        public IEnumerator LowerHand(float seconds = 0.4f) => MoveHand(() => Lowered, seconds);

        private static readonly Vector3 Lowered = new(0.2f, -0.7f, -0.3f);

        private Quaternion BodyRotation => Quaternion.Euler(0f, Phone.eulerAngles.y, 0f);

        private IEnumerator MoveHand(System.Func<Vector3> bodyOffset, float seconds)
        {
            if (_hand == null)
            {
                _hand = Lowered;
                InputSystem.onAfterUpdate += InjectHand;
            }
            var from = _hand.Value;
            for (var t = 0f; t < seconds; t += Time.unscaledDeltaTime)
            {
                _hand = Vector3.Lerp(from, bodyOffset(), Mathf.SmoothStep(0f, 1f, t / seconds));
                yield return null;
            }
            _hand = bodyOffset();
            yield return null;
            yield return null;
        }

        // Re-sends the hand every input update, as Vision does each camera frame.
        private void InjectHand()
        {
            var tracker = HoloKitHandTracker.Instance;
            if (_hand == null || tracker == null || Camera.main == null) return;
            var body = BodyRotation;
            var palm = Phone.position + body * _hand.Value;
            var pointing = _hand.Value.y < -0.5f ? Vector3.down : body * Vector3.forward;
            tracker.InjectPoseForDeviceTest(OpenHand(palm, pointing));
        }

        /// <summary>An open hand: palm (middle MCP) at <paramref name="palm"/>, fingers along <paramref name="pointing"/>.</summary>
        private static Vector3[] OpenHand(Vector3 palm, Vector3 pointing)
        {
            var forward = pointing.normalized;
            var side = Vector3.Cross(Mathf.Abs(Vector3.Dot(forward, Vector3.up)) > 0.9f ? Vector3.forward : Vector3.up, forward).normalized;
            var joints = new Vector3[21];
            joints[(int)HandJoint.Wrist] = palm - forward * 0.08f;
            // Thumb: from the base of the palm, angled away from the fingers.
            var thumb = (forward - side).normalized;
            var cmc = joints[(int)HandJoint.Wrist] + forward * 0.03f - side * 0.025f;
            joints[(int)HandJoint.ThumbCMC] = cmc;
            joints[(int)HandJoint.ThumbMP] = cmc + thumb * 0.03f;
            joints[(int)HandJoint.ThumbIP] = cmc + thumb * 0.055f;
            joints[(int)HandJoint.ThumbTip] = cmc + thumb * 0.08f;
            // Fingers: knuckles across the palm, then three segments forward.
            var fingers = new[] { (HandJoint.IndexMCP, -0.022f, 1f), (HandJoint.MiddleMCP, 0f, 1.05f), (HandJoint.RingMCP, 0.02f, 1f), (HandJoint.LittleMCP, 0.038f, 0.8f) };
            foreach (var (mcp, across, length) in fingers)
            {
                var knuckle = palm + side * across;
                joints[(int)mcp] = knuckle;
                joints[(int)mcp + 1] = knuckle + forward * 0.04f * length;
                joints[(int)mcp + 2] = knuckle + forward * 0.065f * length;
                joints[(int)mcp + 3] = knuckle + forward * 0.085f * length;
            }
            return joints;
        }

        private static IEnumerator WaitForPhone(System.Func<bool> arrived, string failure)
        {
            for (var t = 0f; !arrived() && t < ArrivalTimeout; t += Time.unscaledDeltaTime) yield return null;
            Assert.That(arrived(), Is.True, failure);
        }
    }
}
