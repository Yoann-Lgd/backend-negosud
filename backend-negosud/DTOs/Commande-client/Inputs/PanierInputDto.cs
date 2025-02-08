namespace backend_negosud.DTOs.Commande_client;

public class PanierInputDto
{
    public int ClientId { get; set; }
    public List<LigneCommandeCreateInputDto> LigneCommandes { get; set; } = new List<LigneCommandeCreateInputDto>();
}