using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Chamadum
{
    public int Id { get; set; }

    public int StatusCallId { get; set; }

    public string? CallerName { get; set; }

    public string? CallerNumber { get; set; }

    public string ReportedType { get; set; } = null!;

    public string? Descricao { get; set; }

    public int? LocationId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? AnsweredAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int? LinkedIncidentId { get; set; }

    public virtual Incidente? LinkedIncident { get; set; }

    public virtual Cordenadum? Location { get; set; }

    public virtual StatusChamadum StatusCall { get; set; } = null!;
}
