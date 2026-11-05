using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class IncidentsController(MainContext db) : ApiServiceBase(db)
{
    [HttpGet("incidents")]
    public async Task<ActionResult> ListIncidents(
        string? q,
        string? id,
        string? type,
        string? status,
        string? severity,
        string? description,
        bool includeResolved = false,
        CancellationToken ct = default
    )
    {
        await AdvanceAsync(ct);
        var query = Incidents();

        if (id is not null)
        {
            if (!int.TryParse(id, out var numericId))
            {
                return Error(
                    400,
                    "O id do incidente deve ser um número inteiro válido.",
                    "invalid_id"
                );
            }

            query = query.Where(x => x.Id == numericId);
        }

        if (!includeResolved)
        {
            query = query.Where(x => x.Status.Incidente.ToLower() != "resolved");
        }

        if (type is not null)
        {
            query = query.Where(x => x.Type.TipoIncidente1.ToLower() == type.ToLower());
        }

        if (status is not null)
        {
            query = query.Where(x => x.Status.Incidente.ToLower() == status.ToLower());
        }

        if (severity is not null)
        {
            query = query.Where(x => x.Severity.Severidade1.ToLower() == severity.ToLower());
        }

        if (description is not null)
        {
            query = query.Where(x => x.Descricao != null && x.Descricao.Contains(description));
        }

        if (q is not null)
        {
            var value = q.ToLower();
            query = query.Where(x =>
                x.Id.ToString().Contains(value)
                || x.Type.TipoIncidente1.ToLower().Contains(value)
                || x.Status.Incidente.ToLower().Contains(value)
                || (x.Descricao ?? "").ToLower().Contains(value)
            );
        }

        var incidents = await query.ToListAsync(ct);
        var units = await Units().ToListAsync(ct);
        var attended = await db.IncidenteUnidadeAtendimentos.AsNoTracking().ToListAsync(ct);
        return Ok(incidents.Select(x => Incident(x, units, attended)).ToList());
    }

    [HttpGet("incidents/{incidentId:int}")]
    public async Task<ActionResult> GetIncident(int incidentId, CancellationToken ct)
    {
        await AdvanceAsync(ct);
        var result = await IncidentById(incidentId, ct);

        if (result is null)
        {
            return Error(404, "O incidente não existe.", "incident_not_found");
        }

        return Ok(result);
    }

    [HttpPost("incidents")]
    public async Task<ActionResult> CreateIncident(IncidentCreate input, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Type) || !Valid(input.Location))
        {
            return Error(
                400,
                "O tipo e uma localização válida são obrigatórios.",
                "validation_error"
            );
        }

        var type = await db.TipoIncidentes.SingleOrDefaultAsync(
            x => x.TipoIncidente1.ToLower() == input.Type.ToLower(),
            ct
        );

        if (type is null)
        {
            return Error(400, "O tipo de incidente é inválido.", "invalid_type");
        }

        var sub = await db.IncidenteSubTipos.SingleOrDefaultAsync(
            x =>
                x.TipoIncidenteId == type.Id
                && x.SubType.ToLower() == (input.Subtype ?? "unknown").ToLower(),
            ct
        );

        var sev = await db.Severidades.SingleOrDefaultAsync(
            x => x.Severidade1.ToLower() == (input.Severity ?? "medium").ToLower(),
            ct
        );

        if (sub is null || sev is null)
        {
            return Error(400, "O subtipo ou a severidade é inválido.", "validation_error");
        }

        int? callId = null;

        if (input.CallId is not null)
        {
            if (
                !int.TryParse(input.CallId, out var parsed)
                || !await db.Chamada.AnyAsync(x => x.Id == parsed, ct)
            )
            {
                return Error(400, "A chamada informada não existe.", "invalid_call");
            }

            callId = parsed;
        }

        var states = await IncidentStates(ct);

        if (!states.TryGetValue("unacknowledged", out var unack))
        {
            return Error(
                500,
                "A configuração dos status de incidente está incompleta.",
                "configuration_error"
            );
        }

        var incident = new Incidente
        {
            TypeId = type.Id,
            SubtypeId = sub.Id,
            SeverityId = sev.Id,
            StatusId = unack,
            Location = new Cordenadum { X = input.Location!.X, Y = input.Location.Y },
            Descricao = input.Description,
            CallId = callId,
            ReportedAt = DateTime.UtcNow,
        };

        db.Incidentes.Add(incident);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetIncident),
            new { incidentId = incident.Id },
            await IncidentById(incident.Id, ct)
        );
    }

    [HttpPatch("incidents/{incidentId:int}")]
    public async Task<ActionResult> PatchIncident(
        int incidentId,
        IncidentUpdate input,
        CancellationToken ct
    )
    {
        if (
            input.Type is null
            && input.Subtype is null
            && input.Severity is null
            && input.Location is null
            && input.Description is null
            && input.AssignedUnitIds is null
        )
            return Error(400, "Pelo menos um campo deve ser informado.", "validation_error");
        await AdvanceAsync(ct);
        var incident = await db
            .Incidentes.Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == incidentId, ct);
        if (incident is null)
            return Error(404, "O incidente não existe.", "incident_not_found");
        if (!Is(incident.Status.Incidente, "acknowledged"))
            return Error(
                409,
                "Somente incidentes reconhecidos podem ser editados.",
                "invalid_state"
            );
        if (input.Type is not null)
        {
            var type = await db.TipoIncidentes.SingleOrDefaultAsync(
                x => x.TipoIncidente1.ToLower() == input.Type.ToLower(),
                ct
            );
            if (type is null)
                return Error(400, "O tipo de incidente é inválido.", "invalid_type");
            incident.TypeId = type.Id;
        }
        if (input.Subtype is not null)
        {
            var sub = await db.IncidenteSubTipos.SingleOrDefaultAsync(
                x =>
                    x.TipoIncidenteId == incident.TypeId
                    && x.SubType.ToLower() == input.Subtype.ToLower(),
                ct
            );
            if (sub is null)
                return Error(
                    400,
                    "O subtipo é inválido para o tipo selecionado.",
                    "invalid_subtype"
                );
            incident.SubtypeId = sub.Id;
        }
        if (input.Severity is not null)
        {
            var sev = await db.Severidades.SingleOrDefaultAsync(
                x => x.Severidade1.ToLower() == input.Severity.ToLower(),
                ct
            );
            if (sev is null)
                return Error(400, "A severidade é inválida.", "invalid_severity");
            incident.SeverityId = sev.Id;
        }
        if (input.Location is not null)
        {
            if (!Valid(input.Location))
                return Error(400, "A localização deve estar dentro do mapa.", "invalid_location");
            incident.Location = new Cordenadum { X = input.Location.X, Y = input.Location.Y };
        }
        if (input.Description is not null)
            incident.Descricao = input.Description;
        if (input.AssignedUnitIds is not null)
        {
            var ids = new List<int>();
            foreach (var s in input.AssignedUnitIds.Distinct())
            {
                if (!int.TryParse(s, out var v))
                    return Error(400, "assignedUnitIds deve conter ids válidos.", "invalid_unit");
                ids.Add(v);
            }
            var chosen = await db
                .Unidades.Include(x => x.StatusUnit)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(ct);
            if (
                chosen.Count != ids.Count
                || chosen.Any(x =>
                    x.AssignedIncidentId != null && x.AssignedIncidentId != incidentId
                    || !Is(x.StatusUnit.Unidade, "available") && x.AssignedIncidentId != incidentId
                )
            )
                return Error(
                    400,
                    "Every assigned unit must be available or already assigned to this incident.",
                    "unit_unavailable"
                );
            var current = await db
                .Unidades.Where(x => x.AssignedIncidentId == incidentId)
                .ToListAsync(ct);
            var us = await UnitStates(ct);
            foreach (var x in current.Where(x => !ids.Contains(x.Id)))
            {
                x.AssignedIncidentId = null;
                x.StatusUnitId = us["available"];
            }
            foreach (var x in chosen.Where(x => x.AssignedIncidentId != incidentId))
            {
                x.AssignedIncidentId = incidentId;
                x.StatusUnitId = us["assigned"];
                db.IncidenteUnidadeAtribuida.Add(
                    new()
                    {
                        IncidenteId = incidentId,
                        UnidadeId = x.Id,
                        AtribuidoEm = DateTime.UtcNow,
                    }
                );
            }
            incident.StatusId = (await IncidentStates(ct))[
                ids.Count == 0 ? "acknowledged" : "assigned"
            ];
        }
        await db.SaveChangesAsync(ct);
        return Ok(await IncidentById(incidentId, ct));
    }

    [HttpPost("incidents/{incidentId:int}/acknowledge")]
    public async Task<ActionResult> Acknowledge(int incidentId, CancellationToken ct)
    {
        var i = await db
            .Incidentes.Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == incidentId, ct);
        if (i is null)
            return Error(404, "O incidente não existe.", "incident_not_found");
        if (!Is(i.Status.Incidente, "unacknowledged"))
            return Error(
                409,
                "Only unacknowledged incidents can be acknowledged.",
                "invalid_state"
            );
        i.StatusId = (await IncidentStates(ct))["acknowledged"];
        await db.SaveChangesAsync(ct);
        return Ok(await IncidentById(incidentId, ct));
    }

    [HttpPost("incidents/{incidentId:int}/resolve")]
    public async Task<ActionResult> Resolve(int incidentId, CancellationToken ct)
    {
        await AdvanceAsync(ct);
        var i = await db
            .Incidentes.Include(x => x.Status)
            .SingleOrDefaultAsync(x => x.Id == incidentId, ct);
        if (i is null)
        {
            return Error(404, "O incidente não existe.", "incident_not_found");
        }
        if (!new[] { "assigned", "en_route", "on_scene" }.Any(s => Is(i.Status.Incidente, s)))
            return Error(
                409,
                "This incident cannot be resolved in its current state.",
                "invalid_state"
            );
        var us = await UnitStates(ct);
        foreach (
            var u in await db
                .Unidades.Where(x => x.AssignedIncidentId == incidentId)
                .ToListAsync(ct)
        )
        {
            u.StatusUnitId = us["returning"];
            db.IncidenteUnidadeAtendimentos.Add(
                new()
                {
                    IncidenteId = incidentId,
                    UnidadeId = u.Id,
                    AtendidoEm = DateTime.UtcNow,
                }
            );
        }
        i.StatusId = (await IncidentStates(ct))["resolved"];
        i.ResolvedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(await IncidentById(incidentId, ct));
    }

    [HttpGet("incidents/{incidentId:int}/recommendation")]
    public async Task<ActionResult> Recommendation(int incidentId, CancellationToken ct)
    {
        var incident = await db
            .Incidentes.Include(x => x.Subtype)
            .SingleOrDefaultAsync(x => x.Id == incidentId, ct);
        if (incident is null)
        {
            return Error(404, "O incidente não existe.", "incident_not_found");
        }
        var parts = Composition(incident.Subtype.SubType);
        var units = await Units()
            .Where(x => x.StatusUnit.Unidade.ToLower() == "available")
            .ToListAsync(ct);
        var suggested = parts
            .SelectMany(p =>
                units
                    .Where(u => Is(u.TypeUnit.TipoUnidade1, p.Type))
                    .Take(p.Count)
                    .Select(u => u.Id.ToString())
            )
            .ToList();
        return Ok(new RecommendationDto(N(incident.Subtype.SubType), parts, suggested));
    }

}
