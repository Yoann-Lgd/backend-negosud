namespace backend_negosud.DTOs;

public class StockDetailDto
{
    public int StockId { get; set; }
    public string ArticleReference { get; set; }
    public int Quantite { get; set; }
    public string RefLot { get; set; }
    public int SeuilMinimum { get; set; }
    public bool ReapprovisionnementAuto { get; set; }
}