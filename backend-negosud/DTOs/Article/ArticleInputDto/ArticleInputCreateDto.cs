namespace backend_negosud.DTOs.Article.ArticleInputDto;

public class ArticleInputCreateDto
{
    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public int FamilleId { get; set; }

    public int FournisseurId { get; set; }

    public int TvaId { get; set; }


}