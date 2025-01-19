using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Lier
{
    public int AdresseId { get; set; }

    public int LivraisonId { get; set; }

    public virtual Livraison Livraison { get; set; } = null!;
}
