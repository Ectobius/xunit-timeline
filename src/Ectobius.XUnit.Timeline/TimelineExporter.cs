using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Ectobius.XUnit.Timeline;

public static class TimelineExporter
{
    public static void ExportToFile(string path, IEnumerable<TraceEvent> events)
    {
        var eventList = events.ToList();

        if (eventList.Count > 0)
        {
            var minTs = eventList.Min(e => e.Ts);
            foreach (var e in eventList)
            {
                e.Ts -= minTs;
            }
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(eventList, options);
        File.WriteAllText(path, json);
    }
}
