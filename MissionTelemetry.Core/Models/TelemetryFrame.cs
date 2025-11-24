using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public sealed class TelemetryFrame
    {
        public required long Sequence {  get; init; }
        public required DateTime TimeStamp { get; init; }
        public required Dictionary<string, double> Values { get; init; }
    }
}
