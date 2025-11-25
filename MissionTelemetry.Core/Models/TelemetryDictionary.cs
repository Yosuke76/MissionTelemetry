using System.Text.Json.Serialization;


namespace MissionTelemetry.Core.Models;



public sealed class TelemetryDictionary
{
    public List<ParameterEntry> Parameters { get; set; } = new();
}

public sealed class ParameterEntry
{
    public string Key { get; set; } = "";
    public string? Unit { get; set; }
    public LimitBundle Limits { get; set; } = new();
}

public sealed class LimitBundle
{
    public LimitSpec? Warning { get; set; }
    public LimitSpec? Alarm { get; set; }
}

/// <summary>
/// Nutzt DEINE Comparator-Enum (LessThan / GreaterThan / Between)
/// </summary>
public sealed class LimitSpec
{
    public Comparator Comparator { get; set; }      // <- deine Enum
    public double? Min { get; set; }                // mappt auf AlarmRule.ThresholdMin
    public double? Max { get; set; }                // mappt auf AlarmRule.ThresholdMax
    public bool Latched { get; set; } = false;      // mappt auf AlarmRule.Latched
}
