using backend_negosud.DTOs.Fournisseur.FournisseurOutputDto;

namespace backend_negosud.DTOs.Commande_fournisseur.Outputs;

public class BonCommandeOutputDto
{
    public int BonCommandeId { get; set; }
    public string Status { get; set; } = null!;

    public string Reference { get; set; } = null!;

    public double Prix { get; set; }
    public DateTime DateCreation { get; set; }
    public FournisseurMinimalOutputDto Fournisseur { get; set; } = null!;
    public List<LigneBonCommandeOutputDto> LigneBonCommandes { get; set; } = null!;
}