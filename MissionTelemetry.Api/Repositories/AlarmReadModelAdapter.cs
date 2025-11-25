using System.Collections.ObjectModel;
using System.Linq;
using MissionTelemetry.Core.Models;
using MissionTelemetry.Core.Services;

namespace MissionTelemetry.Api.Repositories;

public sealed class AlarmReadModelAdapter : IAlarmReadModel
{
    private readonly IAlarmManager _manager;

    public AlarmReadModelAdapter(IAlarmManager manager)
    {
        _manager = manager;
    }

    // Gibt eine schreibgeschützte Kopie der aktuellen Alarmliste zurück
    public ReadOnlyCollection<ActiveAlarm> GetActive()
        => new ReadOnlyCollection<ActiveAlarm>(_manager.Active.ToList());

    // Berechnet die höchste Severity aus der aktuellen Liste
    public Severity Highest
        => _manager.Active.Count > 0
           ? _manager.Active.Max(a => a.Severity)
           : Severity.Info;

    // Quittierung durchreichen
    public void Ack(string id) => _manager.Acknowledge(id);
    public void AckAll() => _manager.AcknowledgeAll();
}
