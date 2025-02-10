using System;
using System.Collections.Generic;
using backend_negosud.Models;

namespace backend_negosud.Entities;

public partial class Utilisateur : ISoftDelete
{
    public int UtilisateurId { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public string? Telephone { get; set; }

    public string AccessToken { get; set; } = null!;

    public int RoleId { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>();

    public virtual ICollection<BonCommande> BonCommandes { get; set; } = new List<BonCommande>();

    public virtual ICollection<Inventorier> Inventoriers { get; set; } = new List<Inventorier>();

    public virtual ICollection<ReinitialisationMdp> ReinitialisationMdps { get; set; } = new List<ReinitialisationMdp>();

    public virtual Role Role { get; set; } = null!;
}
