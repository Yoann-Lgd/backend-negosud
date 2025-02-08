namespace backend_negosud.DTOs.Famille;

public class FamilleWithArticlesCreateInputDto
{
    public string Nom { get; set; }

    public List<int> ArticleIds { get; set; } = new List<int>();
}