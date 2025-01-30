namespace backend_negosud.DTOs;

public class LigneCommandeOutputDto
{
    public int LigneCommandeId { get; set; }
    public int ArticleId { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal Total { get; set; } 
}