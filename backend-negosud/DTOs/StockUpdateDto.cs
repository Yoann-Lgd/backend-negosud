using System.ComponentModel.DataAnnotations;

namespace backend_negosud.DTOs;

public class StockUpdateDto
{
    


    public int stockId { get; set; }

    public int nouvelleQuantite { get; set; }
    
    public int utilisateurId { get; set; }

    public string typeModification { get; set; }
}