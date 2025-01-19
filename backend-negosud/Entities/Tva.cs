using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Tva
{
    public int TvaId { get; set; }

    public double Valeur { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
