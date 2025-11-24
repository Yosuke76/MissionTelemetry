using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionTelemetry.Core.Models;
using Timer = System.Timers.Timer;

namespace MissionTelemetry.Core.Services
{
    public sealed class SimulatedProximitySource : IProximitySource
    {
        private readonly Timer _timer;
        private readonly Random _rnd = new();
        private List<ProximityContact> _contacts = new();

        public event EventHandler<ProximitySnapshot>? Snapshot;
        public bool IsRunning => _timer.Enabled;

        public SimulatedProximitySource(double hz = 1.0)
        {
            _timer = new Timer(1000.0 / hz)
            { AutoReset = true };

            _timer.Elapsed += (_, __) => Tick();
            ResetContacts();

        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();    
        
        private void ResetContacts()
        {
            int n = _rnd.Next(1, 4);
            _contacts = Enumerable.Range(1, n).Select(i =>

            {
                double r = 5 + _rnd.NextDouble() * 25;
                double th = _rnd.NextDouble() * Math.PI * 2;
                double x = r * Math.Cos(th);
                double y = r + Math.Sin(th);

                // Bias Richtung Ursprung (Annährung), Jitter

                double toX = -x, toY = -y;
                double len = Math.Sqrt(toX * toX + toY * toY);

                if (len > 1e-6)
                {
                    toX /= len;
                    toY /= len;
                }

                double bias = 0.03 + _rnd.NextDouble() * 0.02;
                double jx = (_rnd.NextDouble() - 0.05) * 0.02;
                double jy = (_rnd.NextDouble() - 0.05) * 0.02;

                return new ProximityContact
                {
                    Id = i,
                    X_km = x,
                    Y_km = y,
                    Vx_kms = toX * bias + jx,
                    Vy_kms = toY * bias + jy


                };
               
            }).ToList();
                                                      
        }

        private void Tick()
        {
            foreach (var c in _contacts)
            {
                c.X_km += c.Vx_kms;     // dt = 1s
                c.Y_km += c.Vy_kms;

                c.Vx_kms += (_rnd.NextDouble() - 0.5) * 0.002;
                c.Vy_kms += (_rnd.NextDouble() - 0.5) * 0.002;
            }

            _contacts = _contacts.Where(c=> Math.Abs(c.X_km) < 40 && Math.Abs(c.Y_km) < 40).ToList();

            if (_contacts.Count < 3 && _rnd.NextDouble() < 0.15)
            { ResetContacts(); }

            Snapshot?.Invoke(this, new ProximitySnapshot

            {
                TimeStamp = DateTime.Now,
                Contacts = _contacts.ToList()

            } );
        }

        public void Dispose()
        { _timer.Stop(); _timer.Dispose(); }
    }
}
