using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public sealed class AlarmEvent
    {
        public required DateTime TimeStamp { get; init; }
        public required string Key { get; init; }
        public required double Value { get; init; }
        public required Severity Severity { get; init; }
        public required string Message { get; init; }   

    }
}
