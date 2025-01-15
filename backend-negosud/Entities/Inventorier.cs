using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Inventorier
{
    public int UtilisateurId { get; set; }

    public int StockId { get; set; }

    public DateTime DateModification { get; set; }

    public string TypeModification { get; set; } = null!;

    public int QuantitePrecedente { get; set; }

    public int QuantitePostModification { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
