namespace backend_negosud.DTOs.Commande_client;

public class LigneCommandeUpdateInputDto
{
    public int LigneCommandeId { get; set; }
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public bool Livree { get; set; }
}