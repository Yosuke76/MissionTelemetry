using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public interface IAlarmEvaluator
    {
        IReadOnlyList<AlarmRule> Rules { get; }
        IEnumerable<AlarmEvent> Evaluate(TelemetryFrame frame); //Anwendung von Rules auf frame

    }
}
