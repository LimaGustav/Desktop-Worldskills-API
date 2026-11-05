using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class CallsController(MainContext db) : ApiServiceBase(db)
{
    [HttpGet("calls")]
    public async Task<ActionResult> Calls(string? status, CancellationToken ct)
    {
        var q = CallsQuery();
        if (status is not null)
        {
            var s = status.ToLower();
            if (!new[] { "ringing", "active", "ended", "declined", "missed" }.Contains(s))
                return Error(400, "O status da chamada é inválido.", "invalid_status");
            q = q.Where(x => x.StatusCall.StatusChamada.ToLower() == s);
        }
        var calls = await q.ToListAsync(ct);
        var ringing = await db
            .Chamada.AsNoTracking()
            .CountAsync(x => x.StatusCall.StatusChamada.ToLower() == "ringing", ct);
        return Ok(new CallsDto(DateTimeOffset.UtcNow, ringing, calls.Select(Call).ToList()));
    }

    [HttpGet("calls/{callId:int}")]
    public async Task<ActionResult> GetCall(int callId, CancellationToken ct)
    {
        var c = await CallsQuery().SingleOrDefaultAsync(x => x.Id == callId, ct);
        return c is null ? Error(404, "A chamada não existe.", "call_not_found") : Ok(Call(c));
    }

    [HttpPost("calls/{callId:int}/accept")]
    public async Task<ActionResult> Accept(int callId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct
        );
        var call = await db
            .Chamada.Include(x => x.StatusCall)
            .SingleOrDefaultAsync(x => x.Id == callId, ct);
        if (call is null)
            return Error(404, "A chamada não existe.", "call_not_found");
        if (!Is(call.StatusCall.StatusChamada, "ringing"))
            return Error(409, "Somente chamadas tocando podem ser aceitas.", "invalid_state");
        if (await db.Chamada.AnyAsync(x => x.StatusCall.StatusChamada.ToLower() == "active", ct))
            return Error(409, "Já existe outra chamada ativa.", "active_call_exists");
        if (call.LocationId is null)
            return Error(
                400,
                "A call without a location cannot create an incident.",
                "missing_location"
            );
        var typeName = new[] { "fire", "crime", "medical" }.Contains(call.ReportedType.ToLower())
            ? call.ReportedType.ToLower()
            : "medical";
        var type = await db.TipoIncidentes.SingleOrDefaultAsync(
            x => x.TipoIncidente1.ToLower() == typeName,
            ct
        );
        var sub = type is null
            ? null
            : await db
                .IncidenteSubTipos.Where(x => x.TipoIncidenteId == type.Id)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(ct);
        var sev = await db.Severidades.SingleOrDefaultAsync(
            x => x.Severidade1.ToLower() == "medium",
            ct
        );
        if (type is null || sub is null || sev is null)
            return Error(
                500,
                "A configuração do incidente está incompleta.",
                "configuration_error"
            );
        var incident = new Incidente
        {
            TypeId = type.Id,
            SubtypeId = sub.Id,
            SeverityId = sev.Id,
            StatusId = (await IncidentStates(ct))["unacknowledged"],
            LocationId = call.LocationId.Value,
            Descricao = call.Descricao,
            CallId = call.Id,
            ReportedAt = DateTime.UtcNow,
        };
        db.Incidentes.Add(incident);
        call.StatusCallId = (await CallStates(ct))["active"];
        call.AnsweredAt = DateTime.UtcNow;
        call.LinkedIncident = incident;
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        var fullCall = await CallsQuery().SingleAsync(x => x.Id == callId, ct);
        return Ok(new AcceptDto(Call(fullCall), (await IncidentById(incident.Id, ct))!));
    }

    [HttpPost("calls/{callId:int}/decline")]
    public Task<ActionResult> Decline(int callId, CancellationToken ct) =>
        ChangeCall(callId, "ringing", "declined", ct);

    [HttpPost("calls/{callId:int}/end")]
    public Task<ActionResult> End(int callId, CancellationToken ct) =>
        ChangeCall(callId, "active", "ended", ct);

    [HttpPost("calls/{callId:int}/link")]
    public async Task<ActionResult> Link(int callId, LinkCall input, CancellationToken ct)
    {
        if (!int.TryParse(input.IncidentId, out var iid))
            return Error(400, "incidentId é obrigatório e deve ser válido.", "validation_error");
        var c = await db
            .Chamada.Include(x => x.StatusCall)
            .SingleOrDefaultAsync(x => x.Id == callId, ct);
        if (c is null)
        {
            return Error(404, "A chamada não existe.", "call_not_found");
        }
        if (!Is(c.StatusCall.StatusChamada, "active"))
            return Error(409, "Somente chamadas ativas podem ser vinculadas.", "invalid_state");
        var i = await db
            .Incidentes.Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == iid, ct);
        if (i is null)
        {
            return Error(404, "O incidente não existe.", "incident_not_found");
        }
        if (Is(i.Status.Incidente, "resolved"))
            return Error(
                409,
                "Chamadas não podem ser vinculadas a incidentes resolvidos.",
                "incident_resolved"
            );
        c.LinkedIncidentId = iid;
        await db.SaveChangesAsync(ct);
        return Ok(Call(await CallsQuery().SingleAsync(x => x.Id == callId, ct)));
    }

    private async Task<ActionResult> ChangeCall(
        int id,
        string from,
        string to,
        CancellationToken ct
    )
    {
        var c = await db
            .Chamada.Include(x => x.StatusCall)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
        if (c is null)
        {
            return Error(404, "A chamada não existe.", "call_not_found");
        }
        if (!Is(c.StatusCall.StatusChamada, from))
            return Error(
                409,
                "A chamada não está no estado necessário para esta operação.",
                "invalid_state"
            );
        c.StatusCallId = (await CallStates(ct))[to];
        c.EndedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(Call(await CallsQuery().SingleAsync(x => x.Id == id, ct)));
    }

}
