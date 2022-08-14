using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CSQR
{
    public class StopWatch
    {
        private Stopwatch watch;
        private Dictionary<string, long> tickRounds;
        private Dictionary<string, long> timeRounds;
        private int round = 0;
        private string name;

        public StopWatch(string name)
        {
            this.name = name;
            watch = new Stopwatch();
            tickRounds = new Dictionary<string, long>();
            timeRounds = new Dictionary<string, long>();
        }

        public void Start()
        {
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
        }

        public void Reset()
        {
            watch.Reset();
            round = 0;
        }

        public void Round()
        {
            this.Round((round++) + "");
        }

        public void Round(string name)
        {
            watch.Stop();
            tickRounds.Add(name, watch.ElapsedTicks);
            timeRounds.Add(name, watch.ElapsedMilliseconds);
            watch.Reset();
            watch.Start();
        }

        public void PrintResults()
        {
            UnityEngine.Debug.Log("StopWatch results (" + name + ") --------");
            long total = 0;
            foreach (KeyValuePair<string, long> r in tickRounds)
            {
                total += r.Value;
            }
            foreach (KeyValuePair<string, long> r in tickRounds)
            {
                UnityEngine.Debug.Log(r.Key + ": " + Mathf.Round(((float)r.Value / total) * 100) + "% (" + r.Value / 1000 + ") - " + timeRounds[r.Key] + "ms");
            }
            UnityEngine.Debug.Log("(" + name + ") ----------");
        }
    }
}