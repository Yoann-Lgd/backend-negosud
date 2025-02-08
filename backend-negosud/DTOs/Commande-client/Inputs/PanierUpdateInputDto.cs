namespace backend_negosud.DTOs.Commande_client;

public class PanierUpdateInputDto
{
    public int CommandId { get; set; }
    public int ClientId { get; set; }
    public List<LigneCommandeUpdateInputDto> LigneCommandes { get; set; } = new List<LigneCommandeUpdateInputDto>();
}