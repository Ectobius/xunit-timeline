using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Ectobius.XUnit.Timeline.Tests
{
    public class TimelineExporterTests : IDisposable
    {
        private readonly string _tempFile;

        public TimelineExporterTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"timeline-test-{Guid.NewGuid()}.json");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Fact]
        public void ExportToFile_WritesValidJson()
        {
            var events = new List<TraceEvent>
            {
                new TraceEvent
                {
                    Name = "Test1",
                    Ts = 1000,
                    Dur = 500,
                    Tid = 7,
                    Args = new Dictionary<string, string> { ["class"] = "MyClass", ["method"] = "Test1" }
                }
            };

            TimelineExporter.ExportToFile(_tempFile, events);

            var json = File.ReadAllText(_tempFile);
            var deserialized = JsonSerializer.Deserialize<List<TraceEvent>>(json);
            Assert.NotNull(deserialized);
            Assert.Single(deserialized);
            Assert.Equal("Test1", deserialized[0].Name);
        }

        [Fact]
        public void ExportToFile_NormalizesTimestamps()
        {
            var events = new List<TraceEvent>
            {
                new TraceEvent { Name = "A", Ts = 5000, Dur = 100, Tid = 1 },
                new TraceEvent { Name = "B", Ts = 7000, Dur = 200, Tid = 2 }
            };

            TimelineExporter.ExportToFile(_tempFile, events);

            var json = File.ReadAllText(_tempFile);
            var deserialized = JsonSerializer.Deserialize<List<TraceEvent>>(json);
            Assert.Equal(0, deserialized[0].Ts);
            Assert.Equal(2000, deserialized[1].Ts);
        }

        [Fact]
        public void ExportToFile_EmptyEvents_WritesEmptyArray()
        {
            TimelineExporter.ExportToFile(_tempFile, new List<TraceEvent>());

            var json = File.ReadAllText(_tempFile);
            Assert.Equal("[]", json);
        }
    }
}
