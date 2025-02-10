using System;
using System.Collections.Generic;
using backend_negosud.Models;

namespace backend_negosud.Entities;

public partial class Client : ISoftDelete
{
    public int ClientId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public bool EstValide { get; set; }

    public string AcessToken { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public virtual ICollection<ReinitialisationMdp> ReinitialisationMdps { get; set; } = new List<ReinitialisationMdp>();
}
