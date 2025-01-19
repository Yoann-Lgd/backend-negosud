using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Stock
{
    public int StockId { get; set; }

    public string RefLot { get; set; } = null!;

    public int Quantite { get; set; }

    public int SeuilMinimum { get; set; }

    public bool ReapprovisionnementAuto { get; set; }

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual ICollection<Inventorier> Inventoriers { get; set; } = new List<Inventorier>();
}
