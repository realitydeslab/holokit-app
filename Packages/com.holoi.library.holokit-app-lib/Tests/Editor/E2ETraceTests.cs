using System;
using System.IO;
using NUnit.Framework;

namespace HoloKit.Tests
{
    /// <summary>The end-to-end trace records events with game-state snapshots and compares them with a baseline.</summary>
    public sealed class E2ETraceTests
    {
        private string _directory;

        [SetUp] public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), "holokit-e2e-trace-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TearDown] public void TearDown()
        {
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, null);
            if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
        }

        private E2ETrace Journey(int secondBuddha)
        {
            var buddha = 0;
            var trace = new E2ETrace("journey", _directory);
            trace.Probe("buddha", () => buddha);
            trace.Probe("started", () => buddha > 0);
            trace.Event("start");
            buddha = 1;
            trace.Event("touched");
            buddha = secondBuddha;
            trace.Event("touched");
            return trace;
        }

        private string Baseline => Path.Combine(_directory, "journey.baseline.json");

        [Test] public void EventsSnapshotEveryProbeInOrder()
        {
            var events = Journey(2).Events;
            Assert.That(events, Has.Count.EqualTo(3));
            Assert.That(events[1].name, Is.EqualTo("touched"));
            Assert.That(events[2].State("buddha"), Is.EqualTo("2"));
            Assert.That(events[0].State("started"), Is.EqualTo("False"));
        }

        [Test] public void AMissingBaselineFailsAndSaysHowToRecordIt()
        {
            var failure = Assert.Throws<AssertionException>(() => Journey(2).AssertMatchesBaseline(Baseline));
            Assert.That(failure.Message, Does.Contain(E2ETrace.UpdateBaselinesVariable));
        }

        [Test] public void RecordingThenReplayingTheSameJourneyMatches()
        {
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, "1");
            Journey(2).AssertMatchesBaseline(Baseline);
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, null);
            Assert.That(File.Exists(Baseline), Is.True);
            Assert.DoesNotThrow(() => Journey(2).AssertMatchesBaseline(Baseline));
        }

        [Test] public void AChangedValueFailsNamingTheEventAndProbe()
        {
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, "1");
            Journey(2).AssertMatchesBaseline(Baseline);
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, null);
            var failure = Assert.Throws<AssertionException>(() => Journey(5).AssertMatchesBaseline(Baseline));
            Assert.That(failure.Message, Does.Contain("event 3 'touched'").And.Contain("buddha").And.Contain("2").And.Contain("5"));
        }

        [Test] public void AMissingOrExtraEventFails()
        {
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, "1");
            Journey(2).AssertMatchesBaseline(Baseline);
            Environment.SetEnvironmentVariable(E2ETrace.UpdateBaselinesVariable, null);
            var longer = Journey(2);
            longer.Event("touched");
            var failure = Assert.Throws<AssertionException>(() => longer.AssertMatchesBaseline(Baseline));
            Assert.That(failure.Message, Does.Contain("4 events").And.Contain("3"));
        }

        [Test] public void SaveWritesTheTimeline()
        {
            var trace = Journey(2);
            var path = trace.Save();
            Assert.That(File.ReadAllText(path), Does.Contain("\"touched\"").And.Contain("buddha"));
        }
    }
}
