namespace backend_negosud.DTOs.Article.ArticleOutputDto;

public class ArticleMinimalOutputDto
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public int FamilleId { get; set; }
}