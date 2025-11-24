using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public sealed class AlarmManager : IAlarmManager
    {
        private readonly Dictionary<string, ActiveAlarm> _map = new();                      //
        private readonly ObservableCollection<ActiveAlarm> _active = new();

        public ReadOnlyObservableCollection<ActiveAlarm> Active { get; }

        public AlarmManager()
        {
            Active = new ReadOnlyObservableCollection<ActiveAlarm>(_active);
        }

        public void RaiseOrUpdate(string key, Severity sev, string message, double value, bool latched)
        {
            string id = $"{key}|{sev}"; // eindeutige Identität eines Alarmtyps
            if (_map.TryGetValue(id, out var aa))
            {
                aa.LastValue = value;
                aa.LastSeen = DateTime.UtcNow;
                aa.Message = message;
                aa.Count++;
                return;
            }

            aa = new ActiveAlarm
            {
                Id = id,
                Key = key,
                Severity = sev,
                Message = message,
                LastValue = value,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                IsLatched = latched,
                IsAcknowledged = false,
                Count = 1
            };
            _map[id] = aa;
            _active.Insert(0, aa); // neueste oben
        }

        public void ClearIfNotLatched(string key)
        {
            
            var toRemove = new List<string>();
            foreach (var kv in _map)
            {
                if (!kv.Value.IsLatched && kv.Value.Key == key)
                    toRemove.Add(kv.Key);
            }
            foreach (var id in toRemove)
            {
                var aa = _map[id];
                _active.Remove(aa);
                _map.Remove(id);
            }
        }

        public void Acknowledge(string id)
        {
            if (!_map.TryGetValue(id, out var aa)) return;
            aa.IsAcknowledged = true;     
            _active.Remove(aa);           
            _map.Remove(id);
        }

        public void AcknowledgeAll()
        {
            _active.Clear();
            _map.Clear();
        }
    }
}
