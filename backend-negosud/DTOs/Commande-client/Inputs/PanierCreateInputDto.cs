namespace backend_negosud.DTOs.Commande_client;

public class PanierCreateInputDto
{
    public DateTime DateCreation { get; set; }
    public int Quantite { get; set; }
    public bool Valide { get; set; }
    public int ClientId { get; set; }
    public List<LigneCommandeCreateInputDto> LigneCommandes { get; set; } = new List<LigneCommandeCreateInputDto>();
}