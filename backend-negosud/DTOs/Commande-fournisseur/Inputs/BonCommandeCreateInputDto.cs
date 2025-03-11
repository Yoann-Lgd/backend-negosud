namespace backend_negosud.DTOs.Commande_fournisseur.Inputs;

public class BonCommandeCreateInputDto
{
    public double Prix { get; set; }
    public int UtilisateurId { get; set; }
    public int FournisseurID { get; set; }
    public List<LigneBonCommandeCreateInputDto> LigneCommandes { get; set; }
}