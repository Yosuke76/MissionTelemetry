using Microsoft.AspNetCore.Mvc;
using MissionTelemetry.Api.Dtos;
using MissionTelemetry.Api.Repositories;

namespace MissionTelemetry.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AlarmsController : ControllerBase
{
    private readonly IAlarmReadModel _readModel;

    public AlarmsController(IAlarmReadModel readModel)
    {
        _readModel = readModel;
    }

    //Aktive Alarme und höchste Stufe (für Ampel)
    [HttpGet]
    public ActionResult<ActiveAlarmsResponse> GetActive()
    {
        var items = _readModel.GetActive();
        var dto = new ActiveAlarmsResponse
        {
            Highest = _readModel.Highest,
            Items = items.Select(a => new ActiveAlarmDto
            {
                Id = a.Id,
                Key = a.Key,
                Severity = a.Severity,
                Message = a.Message,
                Value = a.LastValue,   
                Latched = a.IsLatched,       
                Acknowledged = a.IsAcknowledged,  
                TimeStamp = a.LastSeen         
            }).ToList()
        };
        return Ok(dto);
    }

    //Einzelnen Alarm quittieren
    [HttpPost("ack/{id}")]
    public IActionResult Ack(string id)
    {
        _readModel.Ack(id);
        return NoContent();
    }

    //Alle Alarme quittieren
    [HttpPost("ack-all")]
    public IActionResult AckAll()
    {
        _readModel.AckAll();
        return NoContent();
    }
}
