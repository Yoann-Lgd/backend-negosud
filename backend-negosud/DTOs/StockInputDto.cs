using System.ComponentModel.DataAnnotations;

namespace backend_negosud.DTOs;

public class StockInputDto
{
    /*[Required]*/
    
    //TODO: passer les memes validation dans le validator
    public int ArticleId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantite { get; set; }

    [Required]
    public string RefLot { get; set; }

    [Range(0, int.MaxValue)]
    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }
}