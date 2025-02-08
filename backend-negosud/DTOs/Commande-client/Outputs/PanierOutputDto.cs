namespace backend_negosud.DTOs.Commande_client.Outputs;

public class PanierOutputDto
{
    public int CommandeId { get; set; }
    public DateTime DateCreation { get; set; }
    public bool Valide { get; set; }
    public int ClientId { get; set; }
    public List<LigneCommandeOutputDto> LigneCommandes { get; set; } = new List<LigneCommandeOutputDto>();
}