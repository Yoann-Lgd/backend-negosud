using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Role
{
    public int RoleId { get; set; }

    public string? Nom { get; set; }

    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
