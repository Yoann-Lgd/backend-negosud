using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
