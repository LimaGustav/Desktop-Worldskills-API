using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class UnitsController(MainContext db) : ApiServiceBase(db)
{
    [HttpGet("units")]
    public async Task<ActionResult> GetUnits(
        string? stationId,
        string? status,
        CancellationToken ct
    )
    {
        await AdvanceAsync(ct);
        var q = Units();
        if (stationId is not null)
        {
            if (!int.TryParse(stationId, out var id))
                return Error(400, "stationId deve ser válido.", "invalid_station_id");
            q = q.Where(x => x.EstacaoId == id);
        }
        if (status is not null)
            q = q.Where(x => x.StatusUnit.Unidade.ToLower() == status.ToLower());
        return Ok((await q.ToListAsync(ct)).Select(Unit).ToList());
    }

}
