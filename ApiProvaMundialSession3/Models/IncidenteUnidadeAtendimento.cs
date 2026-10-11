using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class IncidenteUnidadeAtendimento
{
    public int Id { get; set; }

    public int IncidenteId { get; set; }

    public int UnidadeId { get; set; }

    public DateTime? AtendidoEm { get; set; }

    public virtual Incidente Incidente { get; set; } = null!;

    public virtual Unidade Unidade { get; set; } = null!;
}
