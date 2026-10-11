using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class StatusUnidade
{
    public int Id { get; set; }

    public string Unidade { get; set; } = null!;

    public virtual ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
}
