using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace XUnit.Timeline.Tests
{
    public class SampleTestsFixture : IDisposable
    {
        public void Dispose()
        {
            var events = TimelineCollector.GetEvents();
            if (events.Count > 0)
            {
                var path = Path.Combine(
                    AppContext.BaseDirectory,
                    "test-timeline.json");
                TimelineExporter.ExportToFile(path, events);
            }
        }
    }

    [Timeline]
    [Collection("SharedCollector")]
    public class SampleTests : IClassFixture<SampleTestsFixture>
    {
        [Fact]
        public async Task FastTest()
        {
            await Task.Delay(50);
            Assert.True(true);
        }

        [Fact]
        public async Task MediumTest()
        {
            await Task.Delay(150);
            Assert.True(true);
        }

        [Fact]
        public async Task SlowTest()
        {
            await Task.Delay(300);
            Assert.True(true);
        }

        [Fact]
        public async Task AnotherFastTest()
        {
            await Task.Delay(75);
            Assert.True(true);
        }
    }
}
