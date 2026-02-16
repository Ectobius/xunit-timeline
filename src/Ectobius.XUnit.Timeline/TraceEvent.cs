using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ectobius.XUnit.Timeline;

public class TraceEvent
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("cat")]
    public string Cat { get; set; } = "test";

    [JsonPropertyName("ph")]
    public string Ph { get; set; } = "X";

    [JsonPropertyName("ts")]
    public long Ts { get; set; }

    [JsonPropertyName("dur")]
    public long Dur { get; set; }

    [JsonPropertyName("pid")]
    public int Pid { get; set; } = 1;

    [JsonPropertyName("tid")]
    public int Tid { get; set; }

    [JsonPropertyName("args")]
    public Dictionary<string, string> Args { get; set; } = new Dictionary<string, string>();
}
