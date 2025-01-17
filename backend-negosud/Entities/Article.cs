using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public int FamilleId { get; set; }

    public int FournisseurId { get; set; }

    public int TvaId { get; set; }

    public virtual Famille Famille { get; set; } = null!;

    public virtual Fournisseur Fournisseur { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<LigneBonCommande> LigneBonCommandes { get; set; } = new List<LigneBonCommande>();

    public virtual ICollection<LigneCommande> LigneCommandes { get; set; } = new List<LigneCommande>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual Tva Tva { get; set; } = null!;
}
