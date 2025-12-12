using Microsoft.EntityFrameworkCore;
using System.Linq;
using MissionTelemetry.Core.Models;
using MissionTelemetry.Core.Services;
using MissionTelemetry.Persistence;             
using MissionTelemetry.Persistence.Entities; 


namespace MissionTelemetry.Api.Repositories;

// Liest Telemetrie-Frames direkt aus EF Core (MissionDbContext),
// indem TelemetrySamples nach TimeStamp gruppiert und in Frames zurückgebaut werden.

public sealed class EfTelemetryRepository : ITelemetryRepository
{
    private readonly MissionDbContext _db;

    public EfTelemetryRepository(MissionDbContext db)
        => _db =db;

    public void Add(TelemetryFrame frame)
    {
        

    }

    public IReadOnlyList<TelemetryFrame> GetLatest(int take)
        => QueryFrames(skip: 0, take: take);

    public IReadOnlyList<TelemetryFrame> GetRange(int skip, int take)
        => QueryFrames(skip, take);

    public int Count
        => _db.TelemetrySamples
              .AsNoTracking()
              .Select(s => s.TimeStamp)
              .Distinct()
              .Count();

    public void Clear()
    {
        
        _db.TelemetrySamples.ExecuteDelete();
        // Falls Ziel-Framework ExecuteDelete nicht unterstützt: als Fallback:
        // _db.TelemetrySamples.RemoveRange(_db.TelemetrySamples);
        // _db.SaveChanges();
    } 

    

    private IReadOnlyList<TelemetryFrame> QueryFrames(int skip, int take)
    {
        take = Math.Clamp(take, 1, 500);
        skip = Math.Max(0, skip);

        //  die relevanten Zeitstempel bestimmen
        var stamps = _db.TelemetrySamples
            .AsNoTracking()
            .OrderByDescending(s => s.TimeStamp)
            .Select(s => s.TimeStamp)
            .Distinct()
            .Skip(skip)
            .Take(take)
            .ToList();

        if (stamps.Count == 0)
            return Array.Empty<TelemetryFrame>();

        //  alle Samples für diese Zeitstempel laden
        var samples = _db.TelemetrySamples
            .AsNoTracking()
            .Where(s => stamps.Contains(s.TimeStamp))
            .OrderByDescending(s => s.TimeStamp)
            .ThenBy(s => s.Key)
            .ToList();

        //  Frames pro Zeitstempel zusammenbauen (neueste zuerst)
        var frames =
            samples
            .GroupBy(s => s.TimeStamp)
            .OrderByDescending(g => g.Key)
            .Select((g, idx) =>
        {
            var dict = new Dictionary<string, double>(StringComparer.Ordinal);
            foreach (var s in g)
                dict[s.Key] = s.Value; // letztes gewinnt

            return new TelemetryFrame
            {
            Sequence = idx,           
            TimeStamp = g.Key,
            Values = dict
            };
        })
        .ToList();

        return frames;                 

    // public IReadOnlyList<TelemetryFrame> GetLatest(int take)
    // {
    //     take = Math.Clamp(take, 1, 500);

    //     var latestStamps = _db.TelemetrySamples
    //         .AsNoTracking()
    //         .OrderByDescending(s => s.TimeStamp)
    //         .Select(s => s.TimeStamp)
    //         .Distinct()
    //         .Take(take)
    //         .ToList();

    //     if (latestStamps.Count == 0)
    //         return Array.Empty<TelemetryFrame>();

    //     var samples = _db.TelemetrySamples
    //         .AsNoTracking()
    //         .Where(s => latestStamps.Contains(s.TimeStamp))
    //         .OrderByDescending(s => s.Timestamp)
    //         .ThenBy(s => s.TimeStamp)
    //         .ToList();

    //     var frames =
    //         samples
    //         .Groupby(s => s.TimeStamp)
    //         .OrderByDescending(g => g.Key)
    //         .Select(g =>
    //         {
    //             var dict = new Ditionary<string, double>();
    //             foreach (var s in g)
    //             {
    //                 dict[s.Key] = s.Value;
    //             }

    //             return new TelemetryFrame
    //             {
    //                 TimeStamp = g.Key,
    //                 Values = dict
    //             };
    //         })
    //         .ToList();

    //     return frames;                
    // }    
}
}
