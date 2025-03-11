using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class BonCommande
{
    public int BonCommandeId { get; set; }

    public string Status { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public int UtilisateurId { get; set; }
    public int FournisseurId { get; set; }

    public virtual ICollection<LigneBonCommande> LigneBonCommandes { get; set; } = new List<LigneBonCommande>();

    public virtual Utilisateur Utilisateur { get; set; } = null!;
    public virtual Fournisseur Fournisseur { get; set; } = null!;
}
