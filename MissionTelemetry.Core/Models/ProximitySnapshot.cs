using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public sealed class ProximitySnapshot
    {
        public required DateTime TimeStamp { get; init; }
        public required IReadOnlyList<ProximityContact> Contacts { get; init; }      // Sammelsnapshot pro Tick 

    }
}
