﻿using System;
using System.Collections.Generic;
using backend_negosud.Models;

namespace backend_negosud.Entities;

public partial class Commande : ISoftDelete
{
    public int CommandeId { get; set; }

    public DateTime DateCreation { get; set; }
    public DateTime? ExpirationDate { get; set; } 
    public DateTime? DeletedAt { get; set; } 

    public bool Valide { get; set; }

    public int ClientId { get; set; }

    public int? LivraisonId { get; set; } 

    public int? FactureId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Facture? Facture { get; set; } = null!;

    public virtual Facture? FactureNavigation { get; set; }

    public virtual ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();

    public virtual Livraison? Livraison { get; set; } = null!;

    public virtual ICollection<Reglement>? Reglements { get; set; } = new List<Reglement>();
}
