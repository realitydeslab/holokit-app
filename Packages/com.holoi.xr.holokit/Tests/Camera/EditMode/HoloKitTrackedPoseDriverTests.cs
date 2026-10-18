using System.Reflection;
using HoloKit;
using NUnit.Framework;
using UnityEngine;

public class HoloKitTrackedPoseDriverTests
{
    // A scene torn down before its first frame destroys the driver before Start has found its components
    // (NullReferenceException in OnDestroy, MOFA Training on iPhone 17 Pro, 2026-10-18).
    [Test]
    public void DestroyBeforeStart_DoesNotThrow()
    {
        var gameObject = new GameObject("HoloKit Tracked Pose Driver");
        try
        {
            var driver = gameObject.AddComponent<HoloKitTrackedPoseDriver>();
            var onDestroy = typeof(HoloKitTrackedPoseDriver).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(onDestroy, Is.Not.Null);
            Assert.DoesNotThrow(() => onDestroy.Invoke(driver, null));
        }
        finally
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}
