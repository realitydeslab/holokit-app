using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoloKit.Tests
{
    /// <summary>
    /// Records an end-to-end run: named events with a snapshot of every game-state probe, periodic samples,
    /// checkpoint renders and logged errors. <see cref="AssertMatchesBaseline"/> compares the events (names and
    /// snapshots, not times or poses) with a checked-in baseline, so a change in behaviour fails even when every
    /// step assertion still passes. Record or refresh a baseline with HOLOKIT_E2E_UPDATE_BASELINES=1.
    /// </summary>
    public sealed class E2ETrace : IDisposable
    {
        public const string UpdateBaselinesVariable = "HOLOKIT_E2E_UPDATE_BASELINES";

        [Serializable] public sealed class Entry { public string key; public string value; }

        [Serializable] public sealed class TraceEvent
        {
            public string name;
            public float time;
            public string phone;
            public List<Entry> state = new();
            public string State(string probe) => state.FirstOrDefault(e => e.key == probe)?.value;
        }

        [Serializable] public sealed class Sample
        {
            public float time;
            public string phone;
            public List<Entry> state = new();
        }

        [Serializable] public sealed class Checkpoint
        {
            public string name;
            public float time;
            public string image;
            public float magenta;
            public float litCenter;
        }

        [Serializable] private sealed class Document
        {
            public string scenario;
            public List<TraceEvent> events = new();
            public List<Sample> samples = new();
            public List<Checkpoint> checkpoints = new();
            public List<string> errors = new();
        }

        [Serializable] private sealed class BaselineDocument
        {
            public string scenario;
            public List<TraceEvent> events = new();
        }

        private readonly Document _document = new();
        private readonly List<(string name, Func<object> read)> _probes = new();
        private readonly float _start = Time.realtimeSinceStartup;
        private float _sampleInterval;
        private float _nextSample;
        private bool _recording;

        public string Directory { get; }
        public IReadOnlyList<TraceEvent> Events => _document.events;
        public IReadOnlyList<string> Errors => _document.errors;

        public E2ETrace(string scenario, string directory = null)
        {
            _document.scenario = scenario;
            Directory = directory ?? EditorAppDriver.OutputDirectory("E2E-" + scenario);
            System.IO.Directory.CreateDirectory(Directory);
        }

        /// <summary>Where a package keeps a scenario's baseline: Tests/Editor/Baselines/&lt;scenario&gt;.json, on disk.</summary>
        public static string BaselinePath(string packageName, string scenario) =>
            UnityEditor.FileUtil.GetPhysicalPath($"Packages/{packageName}/Tests/Editor/Baselines/{scenario}.json");

        /// <summary>A named piece of game state, read at every event and sample. Return stable values.</summary>
        public void Probe(string name, Func<object> read) => _probes.Add((name, read));

        /// <summary>Starts collecting logged errors and, in Play Mode, a sample every <paramref name="interval"/> seconds.</summary>
        public void StartRecording(float interval = 0.25f)
        {
            if (_recording) return;
            _recording = true;
            _sampleInterval = interval;
            Application.logMessageReceived += OnLog;
            InputSystem.onAfterUpdate += OnUpdate;
        }

        public void Dispose()
        {
            if (!_recording) return;
            _recording = false;
            Application.logMessageReceived -= OnLog;
            InputSystem.onAfterUpdate -= OnUpdate;
        }

        /// <summary>Records a step of the journey with a snapshot of every probe.</summary>
        public void Event(string name)
        {
            var e = new TraceEvent { name = name, time = Now, phone = PhonePose() };
            e.state.AddRange(Snapshot());
            _document.events.Add(e);
            Debug.Log($"[E2E] {_document.scenario} {name} " + string.Join(" ", e.state.Select(s => $"{s.key}={s.value}")));
        }

        /// <summary>Renders the main camera for review and records its error-shader and lit-area measures.</summary>
        public EditorAppDriver.RenderStats Capture(string name)
        {
            var file = $"{_document.checkpoints.Count + 1:00}-{name}.png";
            var stats = EditorAppDriver.RenderMainCamera(Path.Combine(Directory, file));
            _document.checkpoints.Add(new Checkpoint { name = name, time = Now, image = file, magenta = stats.Magenta, litCenter = stats.LitCenter });
            return stats;
        }

        public void AssertNoErrors() =>
            Assert.That(_document.errors, Is.Empty, $"{_document.scenario} logged errors:\n" + string.Join("\n", _document.errors.Take(5)));

        public string Save()
        {
            var path = Path.Combine(Directory, _document.scenario + ".trace.json");
            File.WriteAllText(path, JsonUtility.ToJson(_document, true));
            return path;
        }

        public void AssertMatchesBaseline(string baselinePath)
        {
            if (Environment.GetEnvironmentVariable(UpdateBaselinesVariable) == "1")
            {
                var record = new BaselineDocument { scenario = _document.scenario };
                record.events.AddRange(_document.events.Select(e => new TraceEvent { name = e.name, state = e.state }));
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(baselinePath));
                File.WriteAllText(baselinePath, JsonUtility.ToJson(record, true));
                Debug.Log($"[E2E] Recorded baseline {baselinePath}; review and commit it.");
                return;
            }
            if (!File.Exists(baselinePath))
                Assert.Fail($"No baseline at {baselinePath}. Run once with {UpdateBaselinesVariable}=1 to record it, then review and commit it.");

            var expected = JsonUtility.FromJson<BaselineDocument>(File.ReadAllText(baselinePath)).events;
            var actual = _document.events;
            if (actual.Count != expected.Count)
                Assert.Fail($"{_document.scenario}: the run had {actual.Count} events; the baseline has {expected.Count}.\n" +
                            $"Run:      {string.Join(", ", actual.Select(e => e.name))}\nBaseline: {string.Join(", ", expected.Select(e => e.name))}");
            var differences = new List<string>();
            for (var i = 0; i < actual.Count; i++)
            {
                if (actual[i].name != expected[i].name)
                {
                    differences.Add($"event {i + 1} is '{actual[i].name}' where the baseline has '{expected[i].name}'");
                    continue;
                }
                foreach (var key in actual[i].state.Select(s => s.key).Union(expected[i].state.Select(s => s.key)))
                {
                    var now = actual[i].State(key) ?? "(missing)";
                    var then = expected[i].State(key) ?? "(missing)";
                    if (now != then) differences.Add($"event {i + 1} '{actual[i].name}': {key} is {now}, baseline {then}");
                }
            }
            Assert.That(differences, Is.Empty, $"{_document.scenario} differs from {baselinePath}:\n" + string.Join("\n", differences));
        }

        private float Now => (float)Math.Round(Time.realtimeSinceStartup - _start, 2);

        private IEnumerable<Entry> Snapshot() => _probes.Select(p => new Entry { key = p.name, value = Format(Read(p.read)) });

        private static object Read(Func<object> read)
        {
            try { return read(); }
            catch (Exception exception) { return "error: " + exception.GetType().Name; }
        }

        private static string Format(object value) => value switch
        {
            null => "null",
            float f => f.ToString("0.0", CultureInfo.InvariantCulture),
            double d => d.ToString("0.0", CultureInfo.InvariantCulture),
            Vector3 v => $"({Format(v.x)}, {Format(v.y)}, {Format(v.z)})",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };

        private static string PhonePose()
        {
            var camera = Camera.main;
            return camera == null ? "-" : $"{Format(camera.transform.position)} yaw {Format(camera.transform.eulerAngles.y)}";
        }

        private void OnUpdate()
        {
            if (Time.realtimeSinceStartup < _nextSample) return;
            _nextSample = Time.realtimeSinceStartup + _sampleInterval;
            var sample = new Sample { time = Now, phone = PhonePose() };
            sample.state.AddRange(Snapshot());
            _document.samples.Add(sample);
        }

        private void OnLog(string message, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            // AR Foundation's own simulation reports Editor-only problems; they are not the app's.
            if (stackTrace.Contains("UnityEngine.XR.Simulation.")) return;
            _document.errors.Add($"{Now:0.00}s {message}\n{stackTrace}");
        }
    }
}
