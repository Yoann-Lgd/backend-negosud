using System.ComponentModel.DataAnnotations;

namespace backend_negosud.DTOs;

public class StockUpdateDto
{
    


    public int Quantite { get; set; }

    public string RefLot { get; set; }
    
    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }
}