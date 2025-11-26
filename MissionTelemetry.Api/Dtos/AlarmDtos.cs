using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Api.Dtos
{
    public sealed class ActiveAlarmDto
    {
        public string Id { get; set; } = "";
        public string Key { get; set; } = "";
        public Severity Severity { get; set; }
        public string Message { get; set; } = "";
        public double Value { get; set; }
        public bool Latched { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime TimeStamp { get; set; }

        
        // public DateTime FirstSeen { get; set; }
        // public int Count { get; set; }
    }

    public sealed class ActiveAlarmsResponse
    {
        public Severity Highest { get; set; }
        public List<ActiveAlarmDto> Items { get; set; } = new();
    }
}



