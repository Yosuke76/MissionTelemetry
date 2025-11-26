using Microsoft.AspNetCore.Mvc;
using MissionTelemetry.Api.Dtos;
using MissionTelemetry.Api.Repositories;

namespace MissionTelemetry.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public sealed class ProximityController : ControllerBase
{
    private readonly IProximityRepository _repo;

    public ProximityController(IProximityRepository repo)
    {
        _repo = repo;
    }

    //Gibt neusten Proximity-Snapshot zurück
    [HttpGet]
    public ActionResult<ProximitySnapshotDto?> GetLatest()
    {
        var s = _repo.GetLatest();
        if (s is null) return NotFound();

        var dto = new ProximitySnapshotDto
        {
            TimeStamp = s.TimeStamp,
            Contacts = (s.Contacts ?? Enumerable.Empty<MissionTelemetry.Core.Models.ProximityContact>())
            .Select(MapContact)
            .ToList()
        };
        return Ok(dto);
    }

    private static ProximityContactDto MapContact(MissionTelemetry.Core.Models.ProximityContact c)
    {
        var t = c.GetType();

        string id = GetAsString(t, c, "Id", "ContactId", "TargetId") ?? "";
        double rel = GetAsDouble(t, c, "RelBearingDeg", "RelativeBearingDeg", "BearingDeg", "Bearing", "AngleDeg");
        double rangeKm = GetAsDouble(t, c, "RangeKm", "DistanceKm", "Range_km", "Range", "Distance");
        double cpaKm = GetAsDouble(t, c, "CPA_Dist_km", "CPADistKm", "CpaKm", "CpaDistanceKm", "Cpa");
        double tcpaS = GetAsDouble(t, c, "TCPA_s", "TCPA", "TcpaSec", "TcpaSeconds");

        return new ProximityContactDto
        {
            Id = id,
            RelBearingDeg = rel,
            RangeKm = rangeKm,
            CPA_Dist_km = cpaKm,
            TCPA_s = tcpaS
        };
    }

    private static string? GetAsString(Type t, object o, params string[] names)
    {
        foreach (var n in names)
        {
            var p = t.GetProperty(n);
            if (p is null) continue;
            var v = p.GetValue(o);
            if (v is null) return null;
            return v.ToString();
        }
        return null;
    }

    private static double GetAsDouble(Type t, object o, params string[] names)
    {
        foreach (var n in names)
        {
            var p = t.GetProperty(n);
            if (p is null) continue;
            var v = p.GetValue(o);
            if (v is null) continue;

            // versucht gängige Typen zu konvertieren
            if (v is double d) return d;
            if (v is float f) return f;
            if (v is int i) return i;
            if (v is long l) return l;
            if (v is decimal m) return (double)m;
            if (double.TryParse(v.ToString(), out var parsed)) return parsed;
        }
        return 0.0; // Default falls nichts gefunden
    }

}