namespace backend_negosud.DTOs.Commande_fournisseur.Inputs;

public class LigneBonCommandeUpdateDto
{
    public int LigneBonCommandeId { get; set; }
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public bool Livree { get; set; }
}