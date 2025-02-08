namespace backend_negosud.DTOs.Commande_client;

public class PanierInputDto
{
    public DateTime DateCreation { get; set; }
    public bool Valide { get; set; }
    public int ClientId { get; set; }
    public List<LigneCommandeCreateInputDto> LigneCommandes { get; set; } = new List<LigneCommandeCreateInputDto>();
}