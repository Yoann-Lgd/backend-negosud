using System.ComponentModel.DataAnnotations;

namespace backend_negosud.DTOs;

public class StockInputDto
{

    public int ArticleId { get; set; }


    public int Quantite { get; set; }


    public string RefLot { get; set; }

    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }
}