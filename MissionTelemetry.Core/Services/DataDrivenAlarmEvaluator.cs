using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    /// <summary>
    /// Erzeugt AlarmRule-Objekte aus mission_dict.json und evaluiert wie dein bisheriger AlarmEvaluator.
    /// - Arbeitet NUR mit TelemetryFrame.Values (Dictionary)
    /// - Setzt AlarmEvent.TimeStamp = frame.TimeStamp
    /// - Comparator/ThresholdMin/ThresholdMax/Latched wird sauber befüllt
    /// </summary>
    public sealed class DataDrivenAlarmEvaluator : IAlarmEvaluator
    {
        public IReadOnlyList<AlarmRule> Rules { get; }

        public DataDrivenAlarmEvaluator(TelemetryDictionary dict)
        {
            var rules = new List<AlarmRule>();

            foreach (var p in dict.Parameters)
            {
                // WARNING
                if (p.Limits.Warning is { } w)
                {
                    var ok =
                        (w.Comparator == Comparator.LessThan && w.Min.HasValue) ||
                        (w.Comparator == Comparator.GreaterThan && w.Max.HasValue) ||
                        (w.Comparator == Comparator.Between && w.Min.HasValue && w.Max.HasValue);

                    if (ok)
                    {
                        rules.Add(new AlarmRule
                        {
                            Key = p.Key,
                            Severity = Severity.Warning,
                            Comparator = w.Comparator,
                            // GetValueOrDefault() wandelt double? -> double
                            ThresholdMin = w.Min.GetValueOrDefault(),
                            ThresholdMax = w.Max.GetValueOrDefault(),
                            Latched = w.Latched
                        });
                    }
                }

                // ALARM
                if (p.Limits.Alarm is { } a)
                {
                    var ok =
                        (a.Comparator == Comparator.LessThan && a.Min.HasValue) ||
                        (a.Comparator == Comparator.GreaterThan && a.Max.HasValue) ||
                        (a.Comparator == Comparator.Between && a.Min.HasValue && a.Max.HasValue);

                    if (ok)
                    {
                        rules.Add(new AlarmRule
                        {
                            Key = p.Key,
                            Severity = Severity.Alarm,
                            Comparator = a.Comparator,
                            ThresholdMin = a.Min.GetValueOrDefault(),
                            ThresholdMax = a.Max.GetValueOrDefault(),
                            Latched = a.Latched
                        });
                    }
                }
            }

            Rules = rules.AsReadOnly();
        }


        public IEnumerable<AlarmEvent> Evaluate(TelemetryFrame frame)
        {
            if (frame.Values is null) yield break;

            foreach (var rule in Rules)
            {
                if (!frame.Values.TryGetValue(rule.Key, out var v)) continue;
                if (!rule.IsTriggered(v)) continue;

                yield return new AlarmEvent
                {
                    TimeStamp = frame.TimeStamp,   // <- exakt deine Property
                    Key = rule.Key,
                    Value = v,
                    Severity = rule.Severity,
                    Message = $"{rule.Key} breached ({v})"
                };
            }
        }
    }
}
