using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Client
{
    public int ClientId { get; set; }

    public string Nom { get; set; }

    public string Prenom { get; set; }

    public string Email { get; set; }

    public string MotDePasse { get; set; } = null!;

    public string Tel { get; set; }

    public bool EstValide { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public virtual ICollection<ReinitialisationMdp> ReinitialisationMdps { get; set; } = new List<ReinitialisationMdp>();
}
