using System.ComponentModel.DataAnnotations;

namespace backend_negosud.DTOs;

public class StockUpdateDto
{
    
    //TODO: passer les memes validation dans le validator
    [Range(0, int.MaxValue)]
    public int Quantite { get; set; }

    public string RefLot { get; set; }

    [Range(0, int.MaxValue)]
    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }
}