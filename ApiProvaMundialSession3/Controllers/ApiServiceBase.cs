using System.Data;
using ApiProvaMundialSession3.Contexts;
using ApiProvaMundialSession3.DTO;
using ApiProvaMundialSession3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Controllers;

public abstract class ApiServiceBase(MainContext db) : ControllerBase
{
    protected readonly MainContext db = db;

    protected IQueryable<Incidente> Incidents() =>
        db
            .Incidentes.AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Subtype)
            .Include(x => x.Severity)
            .Include(x => x.Status)
            .Include(x => x.Location);

    protected IQueryable<Unidade> Units() =>
        db
            .Unidades.AsNoTracking()
            .Include(x => x.TypeUnit)
            .Include(x => x.StatusUnit)
            .Include(x => x.Location)
            .Include(x => x.Estacao)
                .ThenInclude(x => x.Location);

    protected IQueryable<Estacao> Stations() =>
        db.Estacaos.AsNoTracking().Include(x => x.TypeUnit).Include(x => x.Location);

    protected IQueryable<Chamadum> CallsQuery() =>
        db.Chamada.AsNoTracking().Include(x => x.StatusCall).Include(x => x.Location);

    protected async Task<IncidentDto?> IncidentById(int id, CancellationToken ct)
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

    protected Task<Dictionary<string, int>> IncidentStates(CancellationToken ct) =>
        db
            .StatusIncidentes.AsNoTracking()
            .ToDictionaryAsync(x => x.Incidente.ToLower(), x => x.Id, ct);

    protected Task<Dictionary<string, int>> UnitStates(CancellationToken ct) =>
        db.StatusUnidades.AsNoTracking().ToDictionaryAsync(x => x.Unidade.ToLower(), x => x.Id, ct);

    protected Task<Dictionary<string, int>> CallStates(CancellationToken ct) =>
        db
            .StatusChamada.AsNoTracking()
            .ToDictionaryAsync(x => x.StatusChamada.ToLower(), x => x.Id, ct);

    protected async Task AdvanceAsync(CancellationToken ct)
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

    protected static IncidentDto Incident(
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

    protected static StationDto Station(Estacao x, IEnumerable<Unidade> u) =>
        new(
            x.Id.ToString(),
            N(x.TypeUnit.TipoUnidade1),
            x.Name,
            C(x.Location),
            u.Count(z => z.EstacaoId == x.Id && Is(z.StatusUnit.Unidade, "available"))
        );

    protected static UnitDto Unit(Unidade x) =>
        new(
            x.Id.ToString(),
            x.EstacaoId.ToString(),
            N(x.TypeUnit.TipoUnidade1),
            x.CallSign,
            N(x.StatusUnit.Unidade),
            C(x.Location),
            x.AssignedIncidentId?.ToString()
        );

    protected static CallDto Call(Chamadum x) =>
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

    protected static Coordinate C(Cordenadum x) => new(x.X, x.Y);

    protected static DateTimeOffset U(DateTime x) => new(DateTime.SpecifyKind(x, DateTimeKind.Utc));

    protected static DateTimeOffset? U(DateTime? x) => x.HasValue ? U(x.Value) : null;

    protected static bool Valid(Coordinate? x) =>
        x is not null && x.X >= 0 && x.X <= 1408 && x.Y >= 0 && x.Y <= 768;

    protected static bool Is(string? x, string y) =>
        string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

    protected static string N(string x) => x.Trim().ToLowerInvariant();

    protected static List<CompositionPart> Composition(string subtype) =>
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

    protected ActionResult Error(int status, string message, string code) =>
        StatusCode(status, new ErrorDto(message, code));

}
