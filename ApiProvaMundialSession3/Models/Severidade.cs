using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Severidade
{
    public int Id { get; set; }

    public string Severidade1 { get; set; } = null!;

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}
