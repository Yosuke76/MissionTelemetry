using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MissionTelemetry.Api.Repositories;
using MissionTelemetry.Core.Models;
using MissionTelemetry.Core.Services;


namespace MissionTelemetry.Api.Services;

    public sealed class SimulationWorker : BackgroundService
    {
    private readonly ITelemtrySource _telemetry;
    private readonly IProximitySource _proximity;
    private readonly ITelemetryRepository _teleRepo;
    private readonly IProximityRepository _proxRepo;
    private readonly IAlarmEvaluator _evaluator;
    private readonly IAlarmManager _alarms;
    private readonly ILogger<SimulationWorker> _log;


    public SimulationWorker(
        ITelemtrySource telemetry,
        IProximitySource proximity,
        ITelemetryRepository teleRepo,
        IProximityRepository proxRepo,
        IAlarmEvaluator evaluator,
        IAlarmManager alarms,
        ILogger<SimulationWorker> log)
    {
        _telemetry = telemetry;
        _proximity = proximity;
        _teleRepo = teleRepo;
        _proxRepo = proxRepo;
        _evaluator = evaluator;
        _alarms = alarms;
        _log = log;

        _telemetry.FrameReceived += OnFrame;
        _proximity.Snapshot += OnSnapshot;
        
    }

    private void OnFrame(object? sender, TelemetryFrame frame)
    {
        _teleRepo.Add(frame);

        if (frame.Values is null) return;

        foreach(var ev in _evaluator.Evaluate(frame))
        {
            _alarms.RaiseOrUpdate(ev.Key, ev.Severity, ev.Message, ev.Value, latched : false);
        }

        foreach(var key in frame.Values.Keys)
        {
            _alarms.ClearIfNotLatched(key);
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

