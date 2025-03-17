namespace backend_negosud.DTOs.Livraison.Inputs;

public class LivraisonInputCommandeDto
{
    public DateTime? DateEstimee { get; set; } = DateTime.UtcNow.AddDays(5);

    public bool Livree { get; set; } = false;
}