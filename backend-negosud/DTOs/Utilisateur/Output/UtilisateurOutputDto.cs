namespace backend_negosud.DTOs;

public class UtilisateurOutputDto
{
    public int Id { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public int RoleId { get; set; }
    public string? RoleNom { get; set; }
    
    public DateTime deleted_at { get; set; }
    /*public ICollection<AdresseDto>? Adresses { get; set; }*/
}