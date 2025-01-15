using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Utilisateur
{
    public int UtilisateurId { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public string MotDePasse { get; set; } = null!;

    public string? Telephone { get; set; }

    public string AccessToken { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<BonCommande> BonCommandes { get; set; } = new List<BonCommande>();

    public virtual ICollection<Inventorier> Inventoriers { get; set; } = new List<Inventorier>();

    public virtual ICollection<ReinitialisationMdp> ReinitialisationMdps { get; set; } = new List<ReinitialisationMdp>();

    public virtual Role Role { get; set; } = null!;
}
