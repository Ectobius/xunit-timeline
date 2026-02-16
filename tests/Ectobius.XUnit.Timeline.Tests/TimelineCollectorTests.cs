using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ectobius.XUnit.Timeline.Tests;

public class TimelineCollectorTests
{
    [Fact]
    public void RecordStartAndEnd_CreatesEvent()
    {
        var id = $"collector-{Guid.NewGuid()}";
        var name = $"MyTest-{id}";
        TimelineCollector.RecordStart(id, name, "MyClass");
        Thread.Sleep(10);
        TimelineCollector.RecordEnd(id);

        var ev = TimelineCollector.GetEvents().Single(e => e.Name == name);
        Assert.Equal("MyClass", ev.Args["class"]);
        Assert.Equal(name, ev.Args["method"]);
        Assert.True(ev.Dur > 0);
        Assert.True(ev.Tid > 0);
    }

    [Fact]
    public void RecordEnd_WithoutStart_DoesNothing()
    {
        var countBefore = TimelineCollector.GetEvents().Count;
        TimelineCollector.RecordEnd($"nonexistent-{Guid.NewGuid()}");
        var countAfter = TimelineCollector.GetEvents().Count;

        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task ConcurrentRecording_CapturesAllEvents()
    {
        var prefix = Guid.NewGuid().ToString();
        var tasks = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
        {
            var id = $"{prefix}-{i}";
            TimelineCollector.RecordStart(id, $"Conc-{prefix}-{i}", "ConcurrentClass");
            Thread.Sleep(5);
            TimelineCollector.RecordEnd(id);
        }));

        await Task.WhenAll(tasks);

        var events = TimelineCollector.GetEvents()
            .Where(e => e.Name.StartsWith($"Conc-{prefix}-"))
            .ToList();
        Assert.Equal(10, events.Count);
    }
}
