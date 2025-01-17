using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class LigneLivraison
{
    public int LigneLivraisonId { get; set; }

    public int Quantite { get; set; }

    public int LivraisonId { get; set; }

    public int LigneBonCommandeId { get; set; }

    public virtual LigneBonCommande LigneBonCommande { get; set; } = null!;

    public virtual Livraison Livraison { get; set; } = null!;
}
