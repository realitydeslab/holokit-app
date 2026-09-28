using NUnit.Framework;
using RealityDesignLab.MOFA.Library.Base;
using RealityDesignLab.MOFA.TheDuel;
using UnityEngine;

namespace HoloKit.Tests
{
    public sealed class MofaPlayerStatsTests
    {
        [Test]
        public void StatsForAPlayerWhoLeftAreEmptyInsteadOfThrowing()
        {
            // The round summary asks for one player per team; a player may have left mid-round.
            var go = new GameObject("Duel Reality Manager");
            try
            {
                var manager = go.AddComponent<MofaDuelRealityManager>();
                var stats = manager.GetPlayerStats(null);
                Assert.That(stats.Kill, Is.Zero);
                Assert.That(stats.Death, Is.Zero);
                Assert.That(stats.HitRate, Is.Zero);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
