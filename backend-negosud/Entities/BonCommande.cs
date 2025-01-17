using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class BonCommande
{
    public int BonCommandeId { get; set; }

    public string Status { get; set; }

    public string Reference { get; set; }

    public double Prix { get; set; }

    public int UtilisateurId { get; set; }

    public virtual ICollection<LigneBonCommande> LigneBonCommandes { get; set; } = new List<LigneBonCommande>();

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
