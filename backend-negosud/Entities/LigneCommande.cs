using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public partial class LigneCommande
{
    public int LigneCommandeId { get; set; }

    public int Quantite { get; set; }

    public int CommandeId { get; set; }

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual Commande Commande { get; set; } = null!;
}
