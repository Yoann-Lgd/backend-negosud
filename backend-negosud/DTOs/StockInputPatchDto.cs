namespace backend_negosud.DTOs;

public class StockInputPatchDto
{
    public int stockId { get; set; }


    public int Quantite { get; set; }


    public string RefLot { get; set; }

    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }
}