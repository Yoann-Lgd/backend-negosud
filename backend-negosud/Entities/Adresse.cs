using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Adresse
{
    public int AdresseId { get; set; }

    public int Numero { get; set; }

    public string Ville { get; set; } = null!;

    public int CodePostal { get; set; }

    public string Departement { get; set; } = null!;

    public int PaysId { get; set; }

    public int? ClientId { get; set; }

    public int? UtilisateurId { get; set; }

    public int? FournisseurId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Fournisseur? Fournisseur { get; set; }

    public virtual Pays Pays { get; set; } = null!;

    public virtual Utilisateur? Utilisateur { get; set; }
}
