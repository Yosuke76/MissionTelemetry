using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Models
{
    public sealed class ProximityContact
    {
        public required int Id { get; init; }
        public double X_km { get; set; }                       //kontakte zum Fahrzeug in 2d, 
        public double Y_km { get; set; }
        public double Vx_kms { get; set; }
        public double Vy_kms { get; set; }

    }
}
