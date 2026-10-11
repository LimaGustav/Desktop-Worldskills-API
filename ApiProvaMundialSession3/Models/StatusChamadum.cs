using System;
using System.Collections.Generic;

namespace ApiProvaMundialSession3.Models;

public partial class StatusChamadum
{
    public int Id { get; set; }

    public string StatusChamada { get; set; } = null!;

    public virtual ICollection<Chamadum> Chamada { get; set; } = new List<Chamadum>();
}
