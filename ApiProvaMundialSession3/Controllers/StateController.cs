using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class StateController(MainContext db) : ApiServiceBase(db)
{
    [HttpGet("health")]
    public ActionResult Health() => Ok(new HealthDto("ok", DateTimeOffset.UtcNow));

    [HttpGet("state")]
    public async Task<ActionResult> State(CancellationToken ct)
    {
        await AdvanceAsync(ct);
        var incidents = await Incidents().ToListAsync(ct);
        var units = await Units().ToListAsync(ct);
        var stations = await Stations().ToListAsync(ct);
        var attended = await db.IncidenteUnidadeAtendimentos.AsNoTracking().ToListAsync(ct);

        return Ok(
            new StateDto(
                DateTimeOffset.UtcNow,
                incidents.Select(x => Incident(x, units, attended)).ToList(),
                stations.Select(x => Station(x, units)).ToList(),
                units.Select(Unit).ToList()
            )
        );
    }

}
