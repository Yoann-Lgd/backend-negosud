namespace backend_negosud.DTOs.Article.ArticleInputDto;

public class ArticleUpdateInputDto
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; }

    public string Reference { get; set; }

    public double Prix { get; set; }

    public int FamilleId { get; set; }

    public int FournisseurId { get; set; }

    public int TvaId { get; set; }
}