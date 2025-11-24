using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public sealed class AlarmEvaluator : IAlarmEvaluator
    {
        public IReadOnlyList<AlarmRule> Rules { get; }

        public AlarmEvaluator()
        {
            Rules = new List<AlarmRule>
            {
            new() { Key="Power.Voltage",      Severity=Severity.Warning, Comparator=Comparator.LessThan,    ThresholdMin=27.5, Latched=false },
            new() { Key="Power.Voltage",      Severity=Severity.Alarm,   Comparator=Comparator.LessThan,    ThresholdMin=27.0, Latched=false },

            new() { Key="Thermal.BoardTemp",  Severity=Severity.Warning, Comparator=Comparator.GreaterThan, ThresholdMax=45.0, Latched=false },
            new() { Key="Thermal.BoardTemp",  Severity=Severity.Alarm,   Comparator=Comparator.GreaterThan, ThresholdMax=50.0, Latched=false },

            new() { Key="Comms.SNR",          Severity=Severity.Warning, Comparator=Comparator.LessThan,    ThresholdMin=12.0, Latched=false },
            new() { Key="Comms.SNR",          Severity=Severity.Alarm,   Comparator=Comparator.LessThan,    ThresholdMin=8.0,  Latched=false },
            };

        }

        public IEnumerable<AlarmEvent> Evaluate(TelemetryFrame frame)       //liefert HK und erzeugt daraus AlarmEvents
        {
            foreach (var rule in Rules)
            {
                if (!frame.Values.TryGetValue(rule.Key, out var v)) continue;
                if(!rule.IsTriggered(v)) continue;

                yield return new AlarmEvent
                {
                    TimeStamp = frame.TimeStamp,
                    Key = rule.Key,
                    Value = v,
                    Severity = rule.Severity,
                    Message = $"{rule.Key} breached ({v})"
                };
            }
        }



    }
}
