namespace backend_negosud.DTOs.Commande_client;

public class PanierUpdateInputDto
{
    public int CommandId { get; set; }
    public int ArticleId { get; set; }
    public int NewQuantite { get; set; }
}
