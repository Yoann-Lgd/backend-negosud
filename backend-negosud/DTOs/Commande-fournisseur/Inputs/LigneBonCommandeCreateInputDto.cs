namespace backend_negosud.DTOs.Commande_fournisseur.Inputs;

public class LigneBonCommandeCreateInputDto
{
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public double PrixUnitaire { get; set; }
}