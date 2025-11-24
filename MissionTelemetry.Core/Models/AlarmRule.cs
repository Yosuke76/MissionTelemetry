using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public enum Severity { Info, Warning, Alarm}
    public enum Comparator { LessThan, GreaterThan, Between}

    public sealed class AlarmRule
    {
        public required string Key { get; init; }                     // Comparator und Severity
        public required Severity Severity { get; init; }
        public required Comparator Comparator { get; init; }
        public double ThresholdMin { get; init; }
        public double ThresholdMax { get; init; }
        public bool Latched { get; init; } = false;

        public bool IsTriggered(double value) => 
        Comparator switch
        {
            Comparator.LessThan => value < ThresholdMin,
            Comparator.GreaterThan => value > ThresholdMax,
            Comparator.Between => value >= ThresholdMin && value <= ThresholdMax,
            _ => false
        };
    }
}
