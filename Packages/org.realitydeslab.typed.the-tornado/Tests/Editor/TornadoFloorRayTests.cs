using NUnit.Framework;
using RealityDesignLab.Typed.TheTornado;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace HoloKit.Tests
{
    public sealed class TornadoFloorRayTests
    {
        private Transform _eye;

        [SetUp] public void CreateEye() => _eye = new GameObject("Center Eye").transform;
        [TearDown] public void DestroyEye() => Object.DestroyImmediate(_eye.gameObject);

        [Test]
        public void TheFloorIsLookedForStraightDownAheadOfThePlayer()
        {
            // Anywhere in the room, looking along +X with the phone tilted down.
            _eye.SetPositionAndRotation(new Vector3(0f, 1.4f, 3f), Quaternion.Euler(30f, 90f, 0f));
            var ray = TypedTheTornadoRealityManager.FloorRay(_eye, 1f);
            Assert.That(ray.origin, Is.EqualTo(new Vector3(1f, 1.4f, 3f)).Using(Vector3EqualityComparer.Instance), "1 m ahead of the player.");
            Assert.That(ray.direction, Is.EqualTo(Vector3.down).Using(Vector3EqualityComparer.Instance));

            // Where the session started: there is still an "ahead".
            _eye.SetPositionAndRotation(new Vector3(0f, 1.4f, 0f), Quaternion.identity);
            Assert.That(TypedTheTornadoRealityManager.FloorRay(_eye, 1f).origin,
                Is.EqualTo(new Vector3(0f, 1.4f, 1f)).Using(Vector3EqualityComparer.Instance), "1 m ahead at the session origin.");
        }
    }
}
