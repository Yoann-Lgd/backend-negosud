

namespace backend_negosud.DTOs;

public class UtilisateurInputDto
{
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    
    public string Nom { get; set; }
    
    public string Prenom { get; set; }
    public required string access_token { get; set; }
    
    public required int RoleId { get; set; }
    public string Role { get; set; } 
}