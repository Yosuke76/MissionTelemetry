using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public sealed class ActiveAlarm
    {
        public required string Id { get; init; }
        public required string Key { get; init; }
        public required Severity Severity { get; init; }

        public required string Message { get; set; }          //sichbarer Alarm: Zeitfenster der Aktivität und wie oft ausgelöst "count"
        public double LastValue { get; set; }

        public DateTime FirstSeen { get; init; }
        public DateTime LastSeen { get; set; }

        public bool IsLatched { get; init; }                
        public bool IsAcknowledged { get; set;}                   //quittierung
        public int Count { get; set; }


    }
}
