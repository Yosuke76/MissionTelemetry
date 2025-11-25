using System.Threading;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Api.Repositories;

public sealed class InMemoryProximityRepository : IProximityRepository
{
    private ProximitySnapshot? _latest;

    public void Set(ProximitySnapshot snapshot)
        => Interlocked.Exchange(ref _latest, snapshot);

    public ProximitySnapshot? GetLatest()
        => _latest;
}
