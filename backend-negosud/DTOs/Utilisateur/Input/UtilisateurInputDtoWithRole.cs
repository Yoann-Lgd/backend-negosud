namespace backend_negosud.DTOs.Utilisateur.Input;

public class UtilisateurInputDtoWithRole
{
    public string Email { get; set; }
   public int utilisateurId { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public int roleId { get; set; }
    public string Role { get; set; }
}