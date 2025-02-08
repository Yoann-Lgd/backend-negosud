using backend_negosud.DTOs.Commande_client.Outputs;

namespace backend_negosud.DTOs.Famille.Outputs;

public class FamilleOutputDto
{
    public int FamilleId { get; set; }
    public string Nom { get; set; }
    public List<ArticleOutputDto> Articles { get; set; }
}