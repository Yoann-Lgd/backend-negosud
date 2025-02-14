using System;
using System.Collections.Generic;
using backend_negosud.Models;

namespace backend_negosud.Entities;

public partial class Fournisseur : ISoftDelete
{
    public int FournisseurId { get; set; }

    public string Nom { get; set; } = null!;

    public string RaisonSociale { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Tel { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
