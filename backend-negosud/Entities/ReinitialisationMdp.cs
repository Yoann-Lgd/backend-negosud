using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class ReinitialisationMdp
{
    public int ReinitialisationMdpId { get; set; }

    public DateTime DateDemande { get; set; }

    public string MotDePasse { get; set; }

    public string ResetToken { get; set; } = null!;

    public int UtilisateurId { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
