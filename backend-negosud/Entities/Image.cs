using System;
using System.Collections.Generic;

namespace backend_negosud.entities;

public class Image
{
    public int ImageId { get; set; }

    public string? Libelle { get; set; }

    public string? Format { get; set; }

    public string? Slug { get; set; }

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
