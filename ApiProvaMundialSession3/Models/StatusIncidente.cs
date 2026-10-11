using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class StatusIncidente
{
    public int Id { get; set; }

    public string Incidente { get; set; } = null!;

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
