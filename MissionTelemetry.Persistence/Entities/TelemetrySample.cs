namespace MissionTelemetry.Persistence.Entities;

public sealed class TelemetrySample
{
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Key { get; set; } = "";
    public double Value { get; set; }
}
