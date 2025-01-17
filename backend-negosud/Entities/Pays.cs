using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Pays
{
    public int PaysId { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();
}
