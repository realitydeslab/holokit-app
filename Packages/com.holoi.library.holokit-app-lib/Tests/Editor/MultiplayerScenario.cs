using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoloKit.Tests
{
    /// <summary>
    /// Coordinates one role of a multiplayer test that runs in several Editors at once
    /// (Tools/run_multiplayer_test.sh). Each Editor runs the same test with HOLOKIT_MP_ROLE set to
    /// host, player or spectator; roles exchange checkpoints as files in HOLOKIT_MP_DIR.
    /// The Editor transport is UnityTransport on 127.0.0.1, so this covers game-level networking,
    /// not the Multipeer Connectivity transport or QR alignment used on iPhones.
    /// </summary>
    public static class MultiplayerScenario
    {
        public const string Host = "host";
        public const string Player = "player";
        public const string Spectator = "spectator";

        public static string Role => Environment.GetEnvironmentVariable("HOLOKIT_MP_ROLE");

        private static string Directory => Environment.GetEnvironmentVariable("HOLOKIT_MP_DIR");

        public static bool IsConfigured => !string.IsNullOrEmpty(Role) && !string.IsNullOrEmpty(Directory);

        public static void RequireConfigured()
        {
            if (!IsConfigured) Assert.Ignore("Multiplayer scenario: run through Tools/run_multiplayer_test.sh.");
        }

        /// <summary>Marks a checkpoint for this role, with an optional text value for the other roles.</summary>
        public static void Publish(string step, string value = "")
        {
            WriteWhole(PathOf(Role, step), value);
            Debug.Log($"[MULTIPLAYER {Role}] {step} {value}");
        }

        // WaitFor returns as soon as a checkpoint file exists, so it must appear with its whole value: a plain
        // WriteAllText creates the file before writing it, and another Editor read "" from it (Hunting journey,
        // 2026-10-18). Write a temporary file, then rename it into place.
        private static void WriteWhole(string path, string value)
        {
            var temporary = path + ".writing";
            File.WriteAllText(temporary, value);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temporary, path);
        }

        public static string Read(string role, string step) => File.ReadAllText(PathOf(role, step));

        /// <summary>Waits for another role's checkpoint; fails early if that role has failed.</summary>
        public static IEnumerator WaitFor(string role, string step, float timeout = 90f)
        {
            for (var t = 0f; !File.Exists(PathOf(role, step)) && t < timeout; t += Time.unscaledDeltaTime)
            {
                if (File.Exists(PathOf(role, "failed")))
                    Assert.Fail($"The {role} Editor failed before '{step}': {Read(role, "failed")}");
                yield return null;
            }
            Assert.That(File.Exists(PathOf(role, step)), Is.True, $"Timed out waiting for {role} to reach '{step}'.");
        }

        /// <summary>
        /// Publishes <paramref name="step"/> and waits until every role has. Roles finish together, so no
        /// Editor leaves the session (despawning its player on the others) while another still checks state.
        /// </summary>
        public static IEnumerator Barrier(string step, float timeout = 120f)
        {
            Publish(step);
            foreach (var role in new[] { Host, Player, Spectator })
                yield return WaitFor(role, step, timeout);
        }

        /// <summary>
        /// Runs a role's coroutine, including nested coroutines, and records the first failure so the
        /// other Editors stop waiting instead of timing out.
        /// </summary>
        public static IEnumerator Guard(IEnumerator body)
        {
            // Errors are collected here rather than failing at once, so errors raised inside AR Foundation's
            // XR Simulation (Editor-only) can be told apart from the game's; the game's fail the role.
            var errors = new List<string>();
            void OnLog(string message, string stackTrace, LogType type)
            {
                if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
                if (stackTrace.Contains("UnityEngine.XR.Simulation.")) return;
                lock (errors) errors.Add(message.Split('\n')[0]);
            }
            LogAssert.ignoreFailingMessages = true;
            Application.logMessageReceivedThreaded += OnLog;
            var stack = new Stack<IEnumerator>();
            stack.Push(body);
            var nextPeerCheck = 0f;
            try
            {
                while (stack.Count > 0)
                {
                    bool more;
                    try
                    {
                        // Stop as soon as another Editor fails: its game session is gone, so this role would only time out.
                        if (Time.realtimeSinceStartup >= nextPeerCheck)
                        {
                            nextPeerCheck = Time.realtimeSinceStartup + 1f;
                            foreach (var role in new[] { Host, Player, Spectator })
                                if (role != Role && File.Exists(PathOf(role, "failed")))
                                    Assert.Fail($"The {role} Editor failed: {Read(role, "failed")}");
                        }
                        more = stack.Peek().MoveNext();
                        if (!more && stack.Count == 1 && errors.Count > 0)
                            Assert.Fail($"{errors.Count} runtime error(s), first: {errors[0]}");
                    }
                    catch (Exception exception)
                    {
                        WriteWhole(PathOf(Role, "failed"), exception.Message);
                        ExceptionDispatchInfo.Capture(exception).Throw();
                        throw;
                    }
                    if (!more)
                    {
                        stack.Pop();
                        continue;
                    }
                    if (stack.Peek().Current is IEnumerator nested) stack.Push(nested);
                    else yield return stack.Peek().Current;
                }
            }
            finally
            {
                Application.logMessageReceivedThreaded -= OnLog;
            }
            Publish("done");
        }

        private static string PathOf(string role, string step) => Path.Combine(Directory, $"{role}-{step}.txt");
    }
}
