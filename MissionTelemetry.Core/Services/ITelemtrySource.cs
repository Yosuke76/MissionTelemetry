using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;

namespace MissionTelemetry.Core.Services
{
    public interface ITelemtrySource : IDisposable
    {
        event EventHandler<TelemetryFrame>? FrameReceived;
        void Start();
        void Stop();
       
        bool IsRunning { get; }


    }
}
