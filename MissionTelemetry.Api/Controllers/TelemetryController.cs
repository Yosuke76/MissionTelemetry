using Microsoft.AspNetCore.Mvc;
using MissionTelemetry.Api.Dtos;
using MissionTelemetry.Api.Repositories;




namespace MissionTelemetry.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public sealed class TelemetryController : ControllerBase
{
    private readonly ITelemetryRepository _repo;

    public TelemetryController(ITelemetryRepository repo) => _repo = repo;

    [HttpGet("latest")]
    public ActionResult<IEnumerable<TelemetryFrameDto>> GetLatest([FromQuery] int take = 100)
    {
        take = Math.Clamp(take, 1, 1000);
        var frames =_repo.GetLatest(take);
        var dtos = frames.Select(f => new TelemetryFrameDto
        {
            TimeStamp = f.TimeStamp,
            Values = f.Values
        });
        return Ok(dtos);
    }

}

