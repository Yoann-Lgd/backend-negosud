using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public partial class LigneLivraison
{
    public int LigneLivraisonId { get; set; }

    public int Quantite { get; set; }

    public int LivraisonId { get; set; }

    public virtual ICollection<LigneBonCommande> LigneBonCommandes { get; set; } = new List<LigneBonCommande>();

    public virtual Livraison Livraison { get; set; } = null!;
}
