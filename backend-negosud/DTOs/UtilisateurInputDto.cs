using backend_negosud.entities;

namespace backend_negosud.DTOs;

public class UtilisateurInputDto
{
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    
    public string access_token { get; set; }
    
    public RoleDto RoleDto { get; set; }
}