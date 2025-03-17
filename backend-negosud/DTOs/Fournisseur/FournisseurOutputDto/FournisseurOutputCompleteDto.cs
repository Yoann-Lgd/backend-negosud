using backend_negosud.DTOs.Adresse.AdresseOutputDto;
using backend_negosud.DTOs.Article.ArticleOutputDto;

namespace backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;

public class FournisseurOutputCompleteDto
{
    public int FournisseurId { get; set; }

    public string Nom { get; set; }

    public string RaisonSociale { get; set; }

    public string Email { get; set; }

    public string Tel { get; set; }
    public AdresseOutputEssentialDto adresse { get; set; }
    public List<ArticleMinimalOutputDto> ArticleMinimalOutputDtos { get; set; }
}