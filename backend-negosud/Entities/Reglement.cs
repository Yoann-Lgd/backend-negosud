using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Reglement
{
    public int ReglementId { get; set; }

    public string Reference { get; set; } = null!;

    public double Montant { get; set; }

    public DateTime Date { get; set; }

    public int CommandeId { get; set; }

    public virtual Commande Commande { get; set; } = null!;
}
