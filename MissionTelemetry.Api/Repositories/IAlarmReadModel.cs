using System.Collections.ObjectModel;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Api.Repositories;

public interface IAlarmReadModel
{
    ReadOnlyCollection<ActiveAlarm> GetActive();
    Severity Highest { get; }
    void Ack(string id);
    void AckAll();
}
