namespace backend_negosud.DTOs.Commande_fournisseur.Inputs;

public class BonCommandeUpdateDto
{
    public string Status { get; set; } = null!;
    public List<LigneBonCommandeUpdateDto> LigneCommandes { get; set; }
    public int UtilisateurId { get; set; }
}