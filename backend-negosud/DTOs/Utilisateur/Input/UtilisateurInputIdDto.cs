using backend_negosud.Entities;

namespace backend_negosud.DTOs.Utilisateur.Input;

public class UtilisateurInputIdDto
{
    public string Email { get; set; }
    
    public string Nom { get; set; }
    
    public string Prenom { get; set; }
    
    
    public required int RoleId { get; set; }
    public int Id { get; set; } 
}