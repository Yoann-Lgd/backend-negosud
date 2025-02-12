using backend_negosud.DTOs.Famille.Outputs;

namespace backend_negosud.DTOs.Commande_client.Outputs;

public class ArticleOutputDto
{
    public int ArticleId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }

    public FamilleMinimalOutputDto Famille { get; set; }

    public int FournisseurId { get; set; }

    public int TvaId { get; set; }
}