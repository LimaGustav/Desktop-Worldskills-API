using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Incidente
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public int SubtypeId { get; set; }

    public int SeverityId { get; set; }

    public int StatusId { get; set; }

    public DateTime ReportedAt { get; set; }

    public int LocationId { get; set; }

    public string? Descricao { get; set; }

    public int? CallId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual ICollection<Chamadum> Chamada { get; set; } = new List<Chamadum>();

    public virtual ICollection<IncidenteUnidadeAtendimento> IncidenteUnidadeAtendimentos { get; set; } = new List<IncidenteUnidadeAtendimento>();

    public virtual ICollection<IncidenteUnidadeAtribuidum> IncidenteUnidadeAtribuida { get; set; } = new List<IncidenteUnidadeAtribuidum>();

    public virtual Cordenadum Location { get; set; } = null!;

    public virtual Severidade Severity { get; set; } = null!;

    public virtual StatusIncidente Status { get; set; } = null!;

    public virtual IncidenteSubTipo Subtype { get; set; } = null!;

    public virtual TipoIncidente Type { get; set; } = null!;

    public virtual ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
}
