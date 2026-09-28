using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using NUnit.Framework;
using UnityEngine;

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
            File.WriteAllText(PathOf(Role, step), value);
            Debug.Log($"[MULTIPLAYER {Role}] {step} {value}");
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
            var stack = new Stack<IEnumerator>();
            stack.Push(body);
            while (stack.Count > 0)
            {
                bool more;
                try
                {
                    more = stack.Peek().MoveNext();
                }
                catch (Exception exception)
                {
                    File.WriteAllText(PathOf(Role, "failed"), exception.Message);
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
            Publish("done");
        }

        private static string PathOf(string role, string step) => Path.Combine(Directory, $"{role}-{step}.txt");
    }
}
