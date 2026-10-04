using System;
using System.Collections;
using NUnit.Framework;

namespace HoloKit.Tests
{
    /// <summary>
    /// Unity abandons outer coroutines when a nested one throws, so their finally/using blocks never run.
    /// EditorAppDriver.Run drives nested coroutines itself and cleans every level up before failing.
    /// </summary>
    public sealed class EditorAppDriverRunTests
    {
        private static IEnumerator Fails()
        {
            yield return null;
            throw new InvalidOperationException("step failed");
        }

        private static IEnumerator Journey(Action cleanedUp)
        {
            try
            {
                yield return null;
                yield return Fails();
                yield return null;
            }
            finally
            {
                cleanedUp();
            }
        }

        [Test] public void AFailingNestedStepStillRunsTheOuterCleanUpAndRethrows()
        {
            var cleaned = false;
            var run = EditorAppDriver.Run(Journey(() => cleaned = true));
            var failure = Assert.Throws<InvalidOperationException>(() => { while (run.MoveNext()) { } });
            Assert.That(failure.Message, Is.EqualTo("step failed"));
            Assert.That(cleaned, Is.True, "The outer coroutine's finally block did not run.");
        }

        [Test] public void ASucceedingJourneyRunsToTheEnd()
        {
            var cleaned = false;
            var steps = 0;
            IEnumerator Count() { steps++; yield return null; steps++; }
            IEnumerator Outer() { try { yield return Count(); yield return Count(); } finally { cleaned = true; } }
            var run = EditorAppDriver.Run(Outer());
            while (run.MoveNext()) { }
            Assert.That(steps, Is.EqualTo(4));
            Assert.That(cleaned, Is.True);
        }
    }
}
