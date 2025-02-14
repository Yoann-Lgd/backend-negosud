namespace backend_negosud.DTOs.Fournisseur.FournisseurInputDto;

public class FournisseurInputMinimal
{
    public int FournisseurId { get; set; }

    public string Nom { get; set; } = null!;

    public string RaisonSociale { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Tel { get; set; } = null!;
}