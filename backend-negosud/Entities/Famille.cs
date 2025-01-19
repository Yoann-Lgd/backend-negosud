using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Famille
{
    public int FamilleId { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
