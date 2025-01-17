using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Stock
{
    public int StockId { get; set; }

    public string RefLot { get; set; }

    public int Quantite { get; set; }

    public double? SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual ICollection<Inventorier> Inventoriers { get; set; } = new List<Inventorier>();
}
