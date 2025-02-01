namespace backend_negosud.DTOs;

public class StockHistoryDto
{
    public DateTime DateModification { get; set; }
    public string TypeModification { get; set; }
    public int QuantitePrecedente { get; set; }
    public int QuantitePostModification { get; set; }
}