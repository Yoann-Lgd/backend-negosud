using System;
using System.Collections.Generic;

namespace backend_negosud.Entities;

public partial class Image
{
    public int ImageId { get; set; }

    public string Libelle { get; set; } = null!;

    public string Format { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
