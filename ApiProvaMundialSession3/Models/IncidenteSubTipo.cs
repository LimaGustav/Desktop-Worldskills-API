using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class IncidenteSubTipo
{
    public int Id { get; set; }

    public int TipoIncidenteId { get; set; }

    public string SubType { get; set; } = null!;

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual TipoIncidente TipoIncidente { get; set; } = null!;
}
