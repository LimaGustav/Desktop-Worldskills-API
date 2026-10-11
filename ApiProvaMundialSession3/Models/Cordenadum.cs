using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class Cordenadum
{
    public int Id { get; set; }

    public decimal X { get; set; }

    public decimal Y { get; set; }

    public virtual ICollection<Chamadum> Chamada { get; set; } = new List<Chamadum>();

    public virtual ICollection<Estacao> Estacaos { get; set; } = new List<Estacao>();

    public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();

    public virtual ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
}
