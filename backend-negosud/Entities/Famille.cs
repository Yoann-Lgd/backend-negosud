using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Famille
{
    public int FamilleId { get; set; }

    public string? Nom { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
