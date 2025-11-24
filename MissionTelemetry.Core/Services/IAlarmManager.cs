using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public interface IAlarmManager
    {
        ReadOnlyObservableCollection<ActiveAlarm> Active { get; }                //führung AlarmListe , operationen für ack, automatisch entfernen 

        void RaiseOrUpdate(string key, Severity sev, string message, double value, bool latched);
        void ClearIfNotLatched(string key);

        void Acknowledge(string id);
        void AcknowledgeAll();
    }
}
