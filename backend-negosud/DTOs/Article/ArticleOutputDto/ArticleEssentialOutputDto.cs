using backend_negosud.DTOs.Famille.Outputs;
using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;
using backend_negosud.DTOs.Tva.TvaOutputDto;

namespace backend_negosud.DTOs.Article.ArticleOutputDto;

public class ArticleEssentialOutputDto
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public FamilleOutputDto Famille { get; set; } = null!;
    public FournisseurOutputCompleteDto Fournisseur { get; set; } = null!;
    public TvaOutputDto Tva { get; set; } = null!;
}