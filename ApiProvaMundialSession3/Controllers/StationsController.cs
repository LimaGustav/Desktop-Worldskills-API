using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class StationsController(MainContext db) : ApiServiceBase(db)
{
    [HttpGet("stations")]
    public async Task<ActionResult> GetStations(CancellationToken ct)
    {
        await AdvanceAsync(ct);
        var u = await Units().ToListAsync(ct);
        return Ok((await Stations().ToListAsync(ct)).Select(x => Station(x, u)).ToList());
    }

}
