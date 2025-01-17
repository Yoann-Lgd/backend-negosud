using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Facture
{
    public int FactureId { get; set; }

    public string Reference { get; set; }

    public DateTime DateFacturation { get; set; }

    public double MontantHt { get; set; }

    public double MontantTtc { get; set; }

    public double MontantTva { get; set; }

    public int ClientId { get; set; }

    public int CommandeId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Commande Commande { get; set; }

    public virtual Commande? CommandeNavigation { get; set; }
}
