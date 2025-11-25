using System.Text.Json.Serialization;

namespace MissionTelemetry.Api.Dtos;


public sealed class TelemetryFrameDto
{
    public DateTime TimeStamp { get; set; }

    public IDictionary<string, double>? Values { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ValueCount => Values?.Count;     // Anzahl der Werte werden weggelassen, wenn null
}

