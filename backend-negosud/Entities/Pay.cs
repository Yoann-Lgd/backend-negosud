using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public partial class Pay
{
    public int PaysId { get; set; }

    public string? Nom { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();
}
