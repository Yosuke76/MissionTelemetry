using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public interface IProximitySource : IDisposable
    {
        event EventHandler<ProximitySnapshot>? Snapshot;
        void Start();
        void Stop();
        bool IsRunning { get; }


    }
}
