using backend_negosud.DTOs.Article.ArticleOutputDto;

namespace backend_negosud.DTOs.Commande_fournisseur.Outputs;

public class LigneBonCommandeOutputDto
{
    public int Quantite { get; set; }
    public double PrixUnitaire { get; set; }
    public ArticleMinimalOutputDto Article { get; set; } = null!;
}