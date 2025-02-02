namespace backend_negosud.DTOs.Commande_client;

public class LigneCommandeCreateInputDto
{
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
}