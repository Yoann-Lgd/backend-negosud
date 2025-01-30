using System;
using System.Collections.Generic;
using backend_negosud.Enums;

namespace backend_negosud.Entities;

public partial class Commande
{
    public int CommandeId { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public int Quantite { get; set; }

    public decimal Total { get; set; }

    public bool Valide { get; set; } = false;

    public StatutCommande Statut { get; set; }

    public int ClientId { get; set; }

    public int? LivraisonId { get; set; }

    public int? FactureId { get; set; }
    public virtual Client Client { get; set; } = null!;
    public virtual Facture? Facture { get; set; }

    public virtual Livraison? Livraison { get; set; }

    public virtual ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();

    public virtual ICollection<Reglement> Reglements { get; set; } = new List<Reglement>();
    
    public Commande()
    {
        Statut = StatutCommande.Panier;
    }
}
