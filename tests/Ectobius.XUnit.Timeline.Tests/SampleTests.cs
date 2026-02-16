using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Ectobius.XUnit.Timeline.Tests;

internal static class TimelineAutoExport
{
    static TimelineAutoExport()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            var events = TimelineCollector.GetEvents();
            if (events.Count > 0)
            {
                var path = Path.Combine(
                    AppContext.BaseDirectory,
                    "test-timeline.json");
                TimelineExporter.ExportToFile(path, events);
            }
        };
    }

    public static void EnsureRegistered() { }
}

[Timeline]
public class SampleSlowTests
{
    public SampleSlowTests() => TimelineAutoExport.EnsureRegistered();

    [Fact]
    public async Task SlowTest()
    {
        await Task.Delay(300);
        Assert.True(true);
    }

    [Fact]
    public async Task MediumTest()
    {
        await Task.Delay(150);
        Assert.True(true);
    }
}

[Timeline]
public class SampleFastTests
{
    public SampleFastTests() => TimelineAutoExport.EnsureRegistered();

    [Fact]
    public async Task FastTest()
    {
        await Task.Delay(50);
        Assert.True(true);
    }

    [Fact]
    public async Task AnotherFastTest()
    {
        await Task.Delay(75);
        Assert.True(true);
    }
}
