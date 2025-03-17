namespace backend_negosud.DTOs.Livraison.Outputs;

public class LivraisonOutputDto
{
    public int LivraisonId { get; set; }
    public DateTime? DateEstimee { get; set; }
    public DateTime? DateLivraison { get; set; }
    public bool Livree { get; set; }
}