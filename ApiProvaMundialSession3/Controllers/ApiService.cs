using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

[ApiController, Route("api")]
public sealed class ApiService(MainContext db) : ControllerBase
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

    [HttpGet("stations")]
    public async Task<ActionResult> GetStations(CancellationToken ct)
    {
        await AdvanceAsync(ct);
        var u = await Units().ToListAsync(ct);
        return Ok((await Stations().ToListAsync(ct)).Select(x => Station(x, u)).ToList());
    }

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

    private IQueryable<Incidente> Incidents() =>
        db
            .Incidentes.AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Subtype)
            .Include(x => x.Severity)
            .Include(x => x.Status)
            .Include(x => x.Location);

    private IQueryable<Unidade> Units() =>
        db
            .Unidades.AsNoTracking()
            .Include(x => x.TypeUnit)
            .Include(x => x.StatusUnit)
            .Include(x => x.Location)
            .Include(x => x.Estacao)
                .ThenInclude(x => x.Location);

    private IQueryable<Estacao> Stations() =>
        db.Estacaos.AsNoTracking().Include(x => x.TypeUnit).Include(x => x.Location);

    private IQueryable<Chamadum> CallsQuery() =>
        db.Chamada.AsNoTracking().Include(x => x.StatusCall).Include(x => x.Location);

    private async Task<IncidentDto?> IncidentById(int id, CancellationToken ct)
    {
        var i = await Incidents().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (i is null)
            return null;
        return Incident(
            i,
            await Units().ToListAsync(ct),
            await db
                .IncidenteUnidadeAtendimentos.AsNoTracking()
                .Where(x => x.IncidenteId == id)
                .ToListAsync(ct)
        );
    }

    private Task<Dictionary<string, int>> IncidentStates(CancellationToken ct) =>
        db
            .StatusIncidentes.AsNoTracking()
            .ToDictionaryAsync(x => x.Incidente.ToLower(), x => x.Id, ct);

    private Task<Dictionary<string, int>> UnitStates(CancellationToken ct) =>
        db.StatusUnidades.AsNoTracking().ToDictionaryAsync(x => x.Unidade.ToLower(), x => x.Id, ct);

    private Task<Dictionary<string, int>> CallStates(CancellationToken ct) =>
        db
            .StatusChamada.AsNoTracking()
            .ToDictionaryAsync(x => x.StatusChamada.ToLower(), x => x.Id, ct);

    private async Task AdvanceAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var incidents = await db
            .Incidentes.Include(x => x.Status)
            .Include(x => x.IncidenteUnidadeAtribuida)
            .ToListAsync(ct);
        var units = await db
            .Unidades.Include(x => x.StatusUnit)
            .Include(x => x.Estacao)
                .ThenInclude(x => x.Location)
            .Include(x => x.Location)
            .ToListAsync(ct);
        var attended = await db.IncidenteUnidadeAtendimentos.ToListAsync(ct);
        var states = await IncidentStates(ct);
        var unitStates = await UnitStates(ct);
        var change = false;
        foreach (var i in incidents)
        {
            var assigned = units.Where(x => x.AssignedIncidentId == i.Id).ToList();
            if (
                (Is(i.Status.Incidente, "assigned") || Is(i.Status.Incidente, "en_route"))
                && assigned.Count > 0
            )
            {
                var since = i
                    .IncidenteUnidadeAtribuida.Where(x =>
                        assigned.Any(u => u.Id == x.UnidadeId) && x.AtribuidoEm.HasValue
                    )
                    .Select(x => x.AtribuidoEm!.Value)
                    .DefaultIfEmpty(now)
                    .Max();
                if (now - DateTime.SpecifyKind(since, DateTimeKind.Utc) >= TimeSpan.FromSeconds(20))
                {
                    i.StatusId = states["on_scene"];
                    foreach (var u in assigned)
                        u.StatusUnitId = unitStates["on_scene"];
                    change = true;
                }
                else if (
                    now - DateTime.SpecifyKind(since, DateTimeKind.Utc)
                    >= TimeSpan.FromSeconds(5)
                )
                {
                    i.StatusId = states["en_route"];
                    foreach (var u in assigned)
                        u.StatusUnitId = unitStates["en_route"];
                    change = true;
                }
            }
        }
        foreach (
            var u in units.Where(x =>
                Is(x.StatusUnit.Unidade, "returning") && x.AssignedIncidentId.HasValue
            )
        )
        {
            var at = attended
                .Where(a => a.IncidenteId == u.AssignedIncidentId && a.UnidadeId == u.Id)
                .Select(a => a.AtendidoEm)
                .FirstOrDefault();
            if (
                at.HasValue
                && now - DateTime.SpecifyKind(at.Value, DateTimeKind.Utc)
                    >= TimeSpan.FromSeconds(10)
            )
            {
                u.AssignedIncidentId = null;
                u.StatusUnitId = unitStates["available"];
                u.Location.X = u.Estacao.Location.X;
                u.Location.Y = u.Estacao.Location.Y;
                change = true;
            }
        }
        if (change)
            await db.SaveChangesAsync(ct);
    }

    private static IncidentDto Incident(
        Incidente x,
        IEnumerable<Unidade> u,
        IEnumerable<IncidenteUnidadeAtendimento> a
    ) =>
        new(
            x.Id.ToString(),
            N(x.Type.TipoIncidente1),
            N(x.Subtype.SubType),
            N(x.Severity.Severidade1),
            N(x.Status.Incidente),
            U(x.ReportedAt),
            C(x.Location),
            x.Descricao,
            u.Where(z => z.AssignedIncidentId == x.Id).Select(z => z.Id.ToString()).ToList(),
            a.Where(z => z.IncidenteId == x.Id)
                .Select(z => z.UnidadeId.ToString())
                .Distinct()
                .ToList(),
            x.CallId?.ToString(),
            U(x.ResolvedAt)
        );

    private static StationDto Station(Estacao x, IEnumerable<Unidade> u) =>
        new(
            x.Id.ToString(),
            N(x.TypeUnit.TipoUnidade1),
            x.Name,
            C(x.Location),
            u.Count(z => z.EstacaoId == x.Id && Is(z.StatusUnit.Unidade, "available"))
        );

    private static UnitDto Unit(Unidade x) =>
        new(
            x.Id.ToString(),
            x.EstacaoId.ToString(),
            N(x.TypeUnit.TipoUnidade1),
            x.CallSign,
            N(x.StatusUnit.Unidade),
            C(x.Location),
            x.AssignedIncidentId?.ToString()
        );

    private static CallDto Call(Chamadum x) =>
        new(
            x.Id.ToString(),
            N(x.StatusCall.StatusChamada),
            x.CallerName,
            x.CallerNumber,
            N(x.ReportedType),
            x.Descricao,
            x.Location is null ? null : C(x.Location),
            U(x.StartedAt),
            U(x.AnsweredAt),
            U(x.EndedAt),
            x.LinkedIncidentId?.ToString()
        );

    private static Coordinate C(Cordenadum x) => new(x.X, x.Y);

    private static DateTimeOffset U(DateTime x) => new(DateTime.SpecifyKind(x, DateTimeKind.Utc));

    private static DateTimeOffset? U(DateTime? x) => x.HasValue ? U(x.Value) : null;

    private static bool Valid(Coordinate? x) =>
        x is not null && x.X >= 0 && x.X <= 1408 && x.Y >= 0 && x.Y <= 768;

    private static bool Is(string? x, string y) =>
        string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

    private static string N(string x) => x.Trim().ToLowerInvariant();

    private static List<CompositionPart> Composition(string subtype) =>
        N(subtype) switch
        {
            "structure_fire" => [new("fire", 2), new("medical", 1)],
            "vehicle_fire" => [new("fire", 1), new("medical", 1)],
            "wildfire" => [new("fire", 2)],
            "gas_leak" => [new("fire", 1), new("medical", 1)],
            "robbery" or "assault" or "burglary" or "disturbance" => [new("police", 2)],
            "traffic_incident" => [new("police", 1), new("medical", 1)],
            "cardiac_arrest" or "trauma" or "respiratory" or "overdose" or "maternity" =>
            [
                new("medical", 2),
            ],
            _ => [new("medical", 1)],
        };

    private ActionResult Error(int status, string message, string code) =>
        StatusCode(status, new ErrorDto(message, code));

    public record Coordinate(decimal X, decimal Y);

    public record IncidentCreate(
        string? Type,
        Coordinate? Location,
        string? Subtype,
        string? Severity,
        string? Description,
        string? CallId
    );

    public record IncidentUpdate(
        string? Type,
        string? Subtype,
        string? Severity,
        Coordinate? Location,
        string? Description,
        List<string>? AssignedUnitIds
    );

    public record LinkCall(string? IncidentId);

    public record ErrorDto(string Message, string Code);

    public record HealthDto(string Status, DateTimeOffset ServerTime);

    public record StateDto(
        DateTimeOffset ServerTime,
        List<IncidentDto> Incidents,
        List<StationDto> Stations,
        List<UnitDto> Units
    );

    public record IncidentDto(
        string Id,
        string Type,
        string Subtype,
        string Severity,
        string Status,
        DateTimeOffset ReportedAt,
        Coordinate Location,
        string? Description,
        List<string> AssignedUnitIds,
        List<string> AttendingUnitIds,
        string? CallId,
        DateTimeOffset? ResolvedAt
    );

    public record StationDto(
        string Id,
        string Type,
        string Name,
        Coordinate Location,
        int AvailableUnitCount
    );

    public record UnitDto(
        string Id,
        string StationId,
        string Type,
        string CallSign,
        string Status,
        Coordinate Position,
        string? AssignedIncidentId
    );

    public record CallDto(
        string Id,
        string Status,
        string? CallerName,
        string? CallerNumber,
        string ReportedType,
        string? Description,
        Coordinate? Location,
        DateTimeOffset StartedAt,
        DateTimeOffset? AnsweredAt,
        DateTimeOffset? EndedAt,
        string? LinkedIncidentId
    );

    public record CallsDto(DateTimeOffset ServerTime, int RingingCount, List<CallDto> Calls);

    public record AcceptDto(CallDto Call, IncidentDto Incident);

    public record CompositionPart(string Type, int Count);

    public record RecommendationDto(
        string Subtype,
        List<CompositionPart> Composition,
        List<string> SuggestedUnitIds
    );
}
