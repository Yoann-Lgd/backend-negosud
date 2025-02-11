namespace backend_negosud.DTOs.Utilisateur.Input;

public class UtilisateurInputJWTDto
{
    public string Email { get; set; }
    public string MotDePasse { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public required string AccessToken { get; set; }
    public string Role { get; set; }
}