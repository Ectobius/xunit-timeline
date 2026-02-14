using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace XUnit.Timeline
{
    public static class TimelineCollector
    {
        private static readonly ConcurrentDictionary<string, TestStartInfo> InFlight = new ConcurrentDictionary<string, TestStartInfo>();
        private static readonly ConcurrentBag<TraceEvent> Completed = new ConcurrentBag<TraceEvent>();

        public static void RecordStart(string testId, string testName, string className)
        {
            var info = new TestStartInfo
            {
                TestName = testName,
                ClassName = className,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                TimestampUs = GetTimestampUs()
            };
            InFlight[testId] = info;
        }

        public static void RecordEnd(string testId)
        {
            var endUs = GetTimestampUs();

            if (!InFlight.TryRemove(testId, out var info))
                return;

            var traceEvent = new TraceEvent
            {
                Name = info.TestName,
                Ts = info.TimestampUs,
                Dur = endUs - info.TimestampUs,
                Tid = info.ThreadId,
                Args = new Dictionary<string, string>
                {
                    ["class"] = info.ClassName,
                    ["method"] = info.TestName
                }
            };

            Completed.Add(traceEvent);
        }

        public static IReadOnlyList<TraceEvent> GetEvents()
        {
            return Completed.ToArray();
        }

        public static void Reset()
        {
            InFlight.Clear();

            while (Completed.TryTake(out _)) { }
        }

        private static long GetTimestampUs()
        {
            return Stopwatch.GetTimestamp() * 1_000_000 / Stopwatch.Frequency;
        }

        private class TestStartInfo
        {
            public string TestName { get; set; }
            public string ClassName { get; set; }
            public int ThreadId { get; set; }
            public long TimestampUs { get; set; }
        }
    }
}
