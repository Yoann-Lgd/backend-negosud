﻿using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class LigneBonCommande
{
    public int LigneBonCommandeId { get; set; }

    public int Quantite { get; set; }

    public double PrixUnitaire { get; set; }

    public int ArticleId { get; set; }

    public int BonCommandeId { get; set; }
    public bool Livree { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual BonCommande BonCommande { get; set; } = null!;

    public virtual LigneLivraison? LigneLivraison { get; set; }
}
