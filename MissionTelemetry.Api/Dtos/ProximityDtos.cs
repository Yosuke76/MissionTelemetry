namespace MissionTelemetry.Api.Dtos;

public sealed class ProximityContactDto
{
    public string Id { get; set; } = "";
    public double RelBearingDeg { get; set; }
    public double RangeKm { get; set; }
    public double CPA_Dist_km { get; set; }
    public double TCPA_s { get; set; }
}

public sealed class ProximitySnapshotDto
{
    public DateTime TimeStamp { get; set; }
    public List<ProximityContactDto> Contacts { get; set; } = new();

}
