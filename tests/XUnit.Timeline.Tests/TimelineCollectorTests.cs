using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace XUnit.Timeline.Tests
{
    [Collection("SharedCollector")]
    public class TimelineCollectorTests : IDisposable
    {
        public TimelineCollectorTests()
        {
            TimelineCollector.Reset();
        }

        public void Dispose()
        {
            TimelineCollector.Reset();
        }

        [Fact]
        public void RecordStartAndEnd_CreatesEvent()
        {
            TimelineCollector.RecordStart("test1", "MyTest", "MyClass");
            Thread.Sleep(10);
            TimelineCollector.RecordEnd("test1");

            var events = TimelineCollector.GetEvents();
            Assert.Single(events);

            var ev = events[0];
            Assert.Equal("MyTest", ev.Name);
            Assert.Equal("MyClass", ev.Args["class"]);
            Assert.Equal("MyTest", ev.Args["method"]);
            Assert.True(ev.Dur > 0);
            Assert.True(ev.Tid > 0);
        }

        [Fact]
        public void RecordEnd_WithoutStart_DoesNothing()
        {
            TimelineCollector.RecordEnd("nonexistent");

            var events = TimelineCollector.GetEvents();
            Assert.Empty(events);
        }

        [Fact]
        public async Task ConcurrentRecording_CapturesAllEvents()
        {
            var tasks = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
            {
                var id = $"test-{i}";
                TimelineCollector.RecordStart(id, $"Test{i}", "ConcurrentClass");
                Thread.Sleep(5);
                TimelineCollector.RecordEnd(id);
            }));

            await Task.WhenAll(tasks);

            var events = TimelineCollector.GetEvents();
            Assert.Equal(10, events.Count);
        }

        [Fact]
        public void Reset_ClearsAllEvents()
        {
            TimelineCollector.RecordStart("test1", "MyTest", "MyClass");
            TimelineCollector.RecordEnd("test1");
            Assert.Single(TimelineCollector.GetEvents());

            TimelineCollector.Reset();

            Assert.Empty(TimelineCollector.GetEvents());
        }
    }
}
