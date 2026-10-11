using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class TipoUnidade
{
    public int Id { get; set; }

    public string TipoUnidade1 { get; set; } = null!;

    public virtual ICollection<Estacao> Estacaos { get; set; } = new List<Estacao>();

    public virtual ICollection<Unidade> Unidades { get; set; } = new List<Unidade>();
}
