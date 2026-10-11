using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Estacao
{
    public int Id { get; set; }

    public int TypeUnitId { get; set; }

    public string Name { get; set; } = null!;

    public int LocationId { get; set; }

    public int AvailableUnitCount { get; set; }

    public virtual Cordenadum Location { get; set; } = null!;

    public virtual TipoUnidade TypeUnit { get; set; } = null!;

    public virtual ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
}
