using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Holoi.Library.HoloKitAppLib;
using NUnit.Framework;
using UnityEngine;

namespace HoloKit.Tests
{
    /// <summary>
    /// End-to-end journey of a reality that one host plays and others watch, in three Editors
    /// (Tools/run_multiplayer_test.sh): the host ("Enter Reality"); a spectator who watches from the start (the
    /// runner's spectator Editor); and a spectator who joins after the host has acted (the runner's player Editor),
    /// as people join a session at different times. Everyone is a simulated person in the test arena; spectators
    /// stand at the sides facing the middle. The host shares where content is; spectators check what they see
    /// against it. Each role's events are compared with Tests/Editor/Baselines/&lt;scenario&gt;-&lt;role&gt;.json.
    /// </summary>
    public sealed class SpectatedJourney
    {
        public const string EarlySpectator = MultiplayerScenario.Spectator;
        public const string LateSpectator = MultiplayerScenario.Player;

        private static readonly Vector3 EarlySpot = new(2.5f, 0f, 0.5f);
        private static readonly Vector3 LateSpot = new(-2.5f, 0f, 0.5f);

        private SpectatedJourney(string scenario)
        {
            Person = SimulatedPerson.Spawn();
            Start = Person.Phone.position;
            Trace = new E2ETrace(scenario);
        }

        public SimulatedPerson Person { get; }
        public E2ETrace Trace { get; }
        /// <summary>Where every phone starts: the middle of the arena, facing +Z.</summary>
        public Vector3 Start { get; }
        public bool IsHost => MultiplayerScenario.Role == MultiplayerScenario.Host;
        public bool IsLate => MultiplayerScenario.Role == LateSpectator;

        /// <summary>
        /// Runs the role this Editor plays. <paramref name="probes"/> registers game-state probes before the first
        /// event; <paramref name="host"/> plays; <paramref name="spectator"/> watches (both spectators; late ones
        /// join after the host has finished acting and should check only what the host shared last).
        /// </summary>
        public static IEnumerator Run(string scene, string scenario, string packageName, Action<E2ETrace> probes,
            Func<SpectatedJourney, IEnumerator> host, Func<SpectatedJourney, IEnumerator> spectator)
        {
            var role = MultiplayerScenario.Role;
            var isHost = role == MultiplayerScenario.Host;
            if (role == LateSpectator) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "acted", 600f);
            else if (!isHost) yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "hosting");
            yield return EditorAppDriver.EnterReality(scene, isHost, isHost ? HoloKitAppPlayerType.Player : HoloKitAppPlayerType.Spectator);
            var app = HoloKitApp.Instance;
            yield return EditorAppDriver.WaitUntil(() => app.MultiplayerManager.LocalPlayer != null, $"The {role} did not join.");
            if (isHost) MultiplayerScenario.Publish("hosting");

            var journey = new SpectatedJourney($"{scenario}-{role}");
            using (journey.Trace)
            {
                probes?.Invoke(journey.Trace);
                journey.Trace.StartRecording();
                journey.Trace.Event("entered");
                if (isHost)
                {
                    yield return MultiplayerScenario.WaitFor(EarlySpectator, "watching");
                    yield return host(journey);
                    MultiplayerScenario.Publish("acted");
                    journey.Trace.Event("acted");
                    // Hold still while the spectators, including the one joining now, check what they see.
                    yield return MultiplayerScenario.WaitFor(EarlySpectator, "checked", 300f);
                    yield return MultiplayerScenario.WaitFor(LateSpectator, "checked", 600f);
                }
                else
                {
                    yield return journey.Person.WalkTo(journey.Start + (role == LateSpectator ? LateSpot : EarlySpot));
                    yield return journey.Person.LookAt(journey.Start + Vector3.forward * 2f);
                    if (role == EarlySpectator) MultiplayerScenario.Publish("watching");
                    journey.Trace.Event("watching");
                    yield return spectator(journey);
                    MultiplayerScenario.Publish("checked");
                    journey.Trace.Event("checked");
                }
                journey.Trace.AssertNoErrors();
                journey.Trace.AssertMatchesBaseline(E2ETrace.BaselinePath(packageName, $"{scenario}-{role}"));
            }
            yield return MultiplayerScenario.Barrier("finished", 600f);
        }

        /// <summary>
        /// Host: shares a position for the spectators. Unless <paramref name="final"/>, waits until the spectator who
        /// is watching has checked it, so the host does not move on while it is being compared. Final values stay
        /// as they are until the late spectator has checked them.
        /// </summary>
        public IEnumerator Share(string name, Vector3 value, bool final = false)
        {
            MultiplayerScenario.Publish("share-" + name, string.Join(",", new[] { value.x, value.y, value.z }
                .Select(v => v.ToString("R", CultureInfo.InvariantCulture))));
            if (!final) yield return MultiplayerScenario.WaitFor(EarlySpectator, "seen-" + name, 60f);
        }

        /// <summary>Spectator: waits until what it sees is within <paramref name="tolerance"/> of the host's value.</summary>
        public IEnumerator Expect(string name, Func<Vector3> seen, float tolerance, float timeout = 5f)
        {
            yield return MultiplayerScenario.WaitFor(MultiplayerScenario.Host, "share-" + name, 300f);
            var parts = MultiplayerScenario.Read(MultiplayerScenario.Host, "share-" + name).Split(',')
                .Select(p => float.Parse(p, CultureInfo.InvariantCulture)).ToArray();
            var shared = new Vector3(parts[0], parts[1], parts[2]);
            yield return EditorAppDriver.WaitUntilRendering(() => Vector3.Distance(seen(), shared) <= tolerance,
                $"The {MultiplayerScenario.Role} sees {name} at {seen()}, the host at {shared} (tolerance {tolerance} m).", timeout);
            if (!IsLate) MultiplayerScenario.Publish("seen-" + name);
        }

        /// <summary>A GameObject of the reality scene by name, also when inactive.</summary>
        public static GameObject Find(string name)
        {
            var found = Resources.FindObjectsOfTypeAll<Transform>()
                .Where(t => t.name == name && t.gameObject.scene.IsValid() && t.gameObject.scene.isLoaded)
                .Select(t => t.gameObject).ToList();
            Assert.That(found, Has.Count.EqualTo(1), $"Expected one '{name}' in the scene.");
            return found[0];
        }

        /// <summary>Horizontal distance, for content that follows a person on the floor plane.</summary>
        public static float Flat(Vector3 a, Vector3 b) => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }
}
