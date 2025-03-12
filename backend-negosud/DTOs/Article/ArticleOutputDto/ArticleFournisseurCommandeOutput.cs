using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;

namespace backend_negosud.DTOs.Article.ArticleOutputDto;

public class ArticleFournisseurCommandeOutput
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }
    public FournisseurMinimalOutputDto Fournisseur { get; set; } = null!;
}