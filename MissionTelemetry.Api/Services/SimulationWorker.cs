using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MissionTelemetry.Api.Repositories;
using MissionTelemetry.Core.Models;
using MissionTelemetry.Core.Services;
using MissionTelemetry.Persistence;
using MissionTelemetry.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MissionTelemetry.Api.Options;


namespace MissionTelemetry.Api.Services;

    public sealed class SimulationWorker : BackgroundService
    {
    private readonly ITelemtrySource _telemetry;
    private readonly IProximitySource _proximity;
    // private readonly ITelemetryRepository _teleRepo;
    private readonly InMemoryTelemetryRepository? _memRepo;
    private readonly bool _useEf;
    private readonly IProximityRepository _proxRepo;
    private readonly IAlarmEvaluator _evaluator;
    private readonly IAlarmManager _alarms;
    private readonly ILogger<SimulationWorker> _log;
    // private readonly IDbContextFactory<MissionDbContext> _dbFactory;
    private readonly IServiceScopeFactory _scopeFactory;



    public SimulationWorker(
        ITelemtrySource telemetry,
        IProximitySource proximity,
        // ITelemetryRepository teleRepo,
        InMemoryTelemetryRepository memRepo,
        IProximityRepository proxRepo,
        IAlarmEvaluator evaluator,
        IAlarmManager alarms,
        // IDbContextFactory<MissionDbContext> dbFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<TelemetryOptions> options,
        ILogger<SimulationWorker> log)
    {
        _telemetry = telemetry;
        _proximity = proximity;
        // _teleRepo = teleRepo;
        _memRepo = memRepo;
        _proxRepo = proxRepo;
        _evaluator = evaluator;
        _alarms = alarms;
        _useEf = options.Value.UseEfRepository;
        _scopeFactory = scopeFactory;
        _log = log;

        // Events abonnieren
        _telemetry.FrameReceived += OnFrame;
        _proximity.Snapshot += OnSnapshot;
        
    }

    private void OnFrame(object? sender, TelemetryFrame frame)
    {
        // Wenn EF NICHT genutzt wird → Memory-Puffer füllen 
        if (!_useEf)
        {
            _memRepo?.Add(frame);
        }

        // Alarme evaluieren + managen 
        foreach (var ev in _evaluator.Evaluate(frame))
            _alarms.RaiseOrUpdate(ev.Key, ev.Severity, ev.Message, ev.Value, latched: false);

        if (frame.Values is not null)
            foreach (var k in frame.Values.Keys)
                _alarms.ClearIfNotLatched(k);

        // Persistenz via EF Core (Scope pro Event) – bleibt IMMER aktiv
        if (frame.Values is { Count: > 0 })
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MissionDbContext>();

            foreach (var (key, val) in frame.Values)
            {
                db.TelemetrySamples.Add(new TelemetrySample
                {
                    TimeStamp = frame.TimeStamp,
                    Key = key,
                    Value = val
                });
            }
            db.SaveChanges();
        }
    }

    private void OnSnapshot(object? sender, ProximitySnapshot snapshot)
        => _proxRepo.Set(snapshot);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("SimulationWorker starting sources..");
        _telemetry.Start();
        _proximity.Start();
        _log.LogInformation("SimulationWorker started.");
        return Task.CompletedTask;
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("SimulationWorker stoping..");
        _telemetry.Stop();
        _proximity.Stop();  
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _telemetry.Dispose();
        _proximity.Dispose();
        base.Dispose();

    }
}

