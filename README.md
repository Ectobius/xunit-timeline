# Ectobius.XUnit.Timeline

A .NET library that collects xUnit test execution data (thread, timing) and outputs it in [Chrome Trace Event Format](https://docs.google.com/document/d/1CvAClvFfyA5R-PhYUmn5OOQtYMH4h6I0nSsKchNAySU/preview) for timeline visualization.

## What it does

When applied to your xUnit tests, the `[Timeline]` attribute records:
- Which thread each test ran on
- When each test started and how long it took

The collected data is exported as JSON that can be visualized in:
- `chrome://tracing` (built into Chrome)
- [Perfetto UI](https://ui.perfetto.dev/) (web-based, no install needed)

This gives you a clear timeline of parallel test execution — useful for understanding test concurrency, finding bottlenecks, and optimizing test suite performance.

## Project Structure

```
xunit-timeline/
├── Ectobius.XUnit.Timeline.slnx
├── src/
│   └── Ectobius.XUnit.Timeline/              # Class library (netstandard2.0)
│       ├── TraceEvent.cs                     # Chrome Trace Event data model
│       ├── TimelineCollector.cs              # Thread-safe static collector
│       ├── TimelineAttribute.cs              # BeforeAfterTestAttribute
│       └── TimelineExporter.cs               # JSON file writer
└── tests/
    └── Ectobius.XUnit.Timeline.Tests/        # xUnit test project
        ├── TimelineCollectorTests.cs
        ├── TimelineExporterTests.cs
        └── SampleTests.cs
```

## Usage

### 1. Add the attribute to a test class

```csharp
using Ectobius.XUnit.Timeline;

[Timeline]
public class MyTests
{
    [Fact]
    public async Task SomeTest()
    {
        // your test code
    }
}
```

Or apply it assembly-wide:

```csharp
[assembly: Timeline]
```

### 2. Export the collected data

After tests run, call the exporter to write the trace file:

```csharp
var events = TimelineCollector.GetEvents();
TimelineExporter.ExportToFile("test-timeline.json", events);
```

### 3. Visualize

Open the generated JSON file in `chrome://tracing` or drag it into [Perfetto UI](https://ui.perfetto.dev/).

## Output Format

Each test becomes a "complete event" (`ph: "X"`) with the thread ID as the lane:

```json
[
  {
    "name": "FastTest",
    "cat": "test",
    "ph": "X",
    "ts": 0,
    "dur": 75563,
    "pid": 1,
    "tid": 15,
    "args": {
      "class": "MyNamespace.MyTests",
      "method": "FastTest"
    }
  }
]
```

## Building and Testing

```bash
dotnet build
dotnet test
```

After running tests, the `SampleTests` class produces a `test-timeline.json` file in the test output directory.
