using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public partial class Fournisseur
{
    public int FournisseurId { get; set; }

    public string? Nom { get; set; }

    public string? RaisonSociale { get; set; }

    public string Email { get; set; } = null!;

    public string? Tel { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
