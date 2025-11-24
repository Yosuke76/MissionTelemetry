using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models;

public sealed class TelemetryDictionary
{
    public List<ParameterEntry> Parameters { get; set; } = new();
}

public sealed class ParameterEntry
{
    public string Key { get; set; } = "";
    public string? Unit { get; set; }
    public LimitBundle Limits { get; set; } = new();
}

public sealed class LimitBundle
{
    public LimitSpec? Warning { get; set; }
    public LimitSpec? Alarm { get; set; }
}

// einfache Vergleichsarten
public enum ComparatorType { LessThan, GreaterThan, Between }

public sealed class LimitSpec
{
    public ComparatorType Comparator { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public double? Hysteresis { get; set; } // Rückkehr-Puffer
    public double? PersistS { get; set; }   // wie lange verletzt, bevor Alarm
}