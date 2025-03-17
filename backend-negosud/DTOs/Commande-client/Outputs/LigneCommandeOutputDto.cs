namespace backend_negosud.DTOs.Commande_client.Outputs;

public class LigneCommandeOutputDto
{
    public int LigneCommandeId { get; set; }
    public int CommandeId { get; set; }
    public int Quantite { get; set; }
    public bool Livree { get; set; }
    public ArticleOutputDto Article { get; set; }
}