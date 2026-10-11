using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class TipoIncidente
{
    public int Id { get; set; }

    public string TipoIncidente1 { get; set; } = null!;

    public virtual ICollection<IncidenteSubTipo> IncidenteSubTipos { get; set; } = new List<IncidenteSubTipo>();

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
