using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Unidade
{
    public int Id { get; set; }

    public int EstacaoId { get; set; }

    public int TypeUnitId { get; set; }

    public string CallSign { get; set; } = null!;

    public int StatusUnitId { get; set; }

    public int LocationId { get; set; }

    public int? AssignedIncidentId { get; set; }

    public virtual Incidente? AssignedIncident { get; set; }

    public virtual Estacao Estacao { get; set; } = null!;

    public virtual ICollection<IncidenteUnidadeAtendimento> IncidenteUnidadeAtendimentos { get; set; } = new List<IncidenteUnidadeAtendimento>();

    public virtual ICollection<IncidenteUnidadeAtribuidum> IncidenteUnidadeAtribuida { get; set; } = new List<IncidenteUnidadeAtribuidum>();

    public virtual Cordenadum Location { get; set; } = null!;

    public virtual StatusUnidade StatusUnit { get; set; } = null!;

    public virtual TipoUnidade TypeUnit { get; set; } = null!;
}
