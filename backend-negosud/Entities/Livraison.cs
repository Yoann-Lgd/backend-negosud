using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Livraison
{
    public int LivraisonId { get; set; }

    public DateTime? DateEstimee { get; set; }

    public DateTime? DateLivraison { get; set; }

    public bool Livree { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual ICollection<LigneLivraison> LigneLivraisons { get; set; } = new List<LigneLivraison>();

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();
}
