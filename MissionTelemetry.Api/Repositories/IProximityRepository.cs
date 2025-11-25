using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Api.Repositories;

public interface IProximityRepository
{
    void Set(ProximitySnapshot snapshot);
    ProximitySnapshot? GetLatest();
}

