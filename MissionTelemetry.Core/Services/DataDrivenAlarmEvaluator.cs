using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services;

public sealed class DataDrivenAlarmEvaluator : IAlarmEvaluator
{
    private readonly Dictionary<string, ParameterEntry> _map;

    private sealed class State
    {
        public DateTime? WarnSince;
        public DateTime? AlarmSince;
        public bool InWarn;
        public bool InAlarm;
    }

    private readonly Dictionary<string, State> _state = new();

    public DataDrivenAlarmEvaluator(TelemetryDictionary dict)
    {
        _map = dict.Parameters.ToDictionary(p => p.Key, p => p, StringComparer.OrdinalIgnoreCase);

        // optional: Rules-Liste nur als Übersicht/Debug
        var rules = new List<AlarmRule>();
        foreach (var p in dict.Parameters)
        {
            if (p.Limits.Warning is not null) rules.Add(new AlarmRule { Key = p.Key, Severity = Severity.Warning });
            if (p.Limits.Alarm is not null) rules.Add(new AlarmRule { Key = p.Key, Severity = Severity.Alarm });
        }
        Rules = rules.AsReadOnly();
    }

    public IReadOnlyList<AlarmRule> Rules { get; }

    public IEnumerable<AlarmEvent> Evaluate(TelemetryFrame frame)
    {
        var now = frame.TimestampUtc;

        foreach (var (key, value) in EnumerateValues(frame))
        {
            if (!_map.TryGetValue(key, out var param)) continue;
            if (!_state.TryGetValue(key, out var st)) _state[key] = st = new State();

            // 1) Alarm prüfen (höchster Schweregrad zuerst)
            if (TryBreach(param.Limits.Alarm, value))
            {
                st.AlarmSince ??= now;
                if (Elapsed(st.AlarmSince, param.Limits.Alarm?.PersistS, now))
                {
                    st.InAlarm = true;
                    st.InWarn = false;
                    st.WarnSince = null;
                    yield return new AlarmEvent
                    {
                        TimestampUtc = now,
                        Key = key,
                        Value = value,
                        Severity = Severity.Alarm,
                        Message = BuildMsg(key, param.Limits.Alarm!, value)
                    };
                }
            }
            else
            {
                st.AlarmSince = null;
            }

            // 2) Warning nur wenn nicht bereits in Alarm
            if (!st.InAlarm && TryBreach(param.Limits.Warning, value))
            {
                st.WarnSince ??= now;
                if (Elapsed(st.WarnSince, param.Limits.Warning?.PersistS, now))
                {
                    st.InWarn = true;
                    yield return new AlarmEvent
                    {
                        Timetamp = now,
                        Key = key,
                        Value = value,
                        Severity = Severity.Warning,
                        Message = BuildMsg(key, param.Limits.Warning!, value)
                    };
                }
            }
            else if (!st.InAlarm)
            {
                st.WarnSince = null;
            }

            // 3) Reset mit Hysterese (zurück in sicheren Bereich)
            if (param.Limits.Alarm is { } al && st.InAlarm && Safe(al, value))
            {
                st.InAlarm = false;
            }
            if (param.Limits.Warning is { } wr && st.InWarn && Safe(wr, value))
            {
                st.InWarn = false;
            }
        }
    }

    // bekannte Properties zusätzlich als Key anbieten
    private static IEnumerable<KeyValuePair<string, double>> EnumerateValues(TelemetryFrame f)
    {
        if (f.Values is not null)
            foreach (var kv in f.Values) yield return kv;

        yield return new("Power.Voltage", f.PowerVoltage);
        yield return new("Power.Current", f.PowerCurrent);
        yield return new("Thermal.BoardTemp", f.BoardTemp);
        yield return new("RF.SNR", f.SNR);
    }

    private static bool TryBreach(LimitSpec? spec, double v)
    {
        if (spec is null) return false;
        return spec.Comparator switch
        {
            ComparatorType.LessThan => spec.Min.HasValue && v < spec.Min.Value,
            ComparatorType.GreaterThan => spec.Max.HasValue && v > spec.Max.Value,
            ComparatorType.Between => spec.Min.HasValue && spec.Max.HasValue && (v < spec.Min.Value || v > spec.Max.Value),
            _ => false
        };
    }

    private static bool Safe(LimitSpec spec, double v)
    {
        var h = spec.Hysteresis ?? 0.0;
        return spec.Comparator switch
        {
            ComparatorType.LessThan => spec.Min.HasValue && v >= spec.Min.Value + h,
            ComparatorType.GreaterThan => spec.Max.HasValue && v <= spec.Max.Value - h,
            ComparatorType.Between => spec.Min.HasValue && spec.Max.HasValue &&
                                          v > spec.Min.Value + h && v < spec.Max.Value - h,
            _ => true
        };
    }

    private static bool Elapsed(DateTime? since, double? persistS, DateTime now)
        => since is not null && (persistS is null || persistS <= 0 || now >= since.Value.AddSeconds(persistS.Value));

    private static string BuildMsg(string key, LimitSpec spec, double v) =>
        spec.Comparator switch
        {
            ComparatorType.LessThan => $"{key} below {spec.Min}",
            ComparatorType.GreaterThan => $"{key} above {spec.Max}",
            ComparatorType.Between => $"{key} outside [{spec.Min},{spec.Max}]",
            _ => $"{key} out of range"
        };
}

