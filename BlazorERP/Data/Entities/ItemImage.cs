using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class ItemImage
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string MimeType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public int Size { get; set; }

    public byte[]? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Item Item { get; set; } = null!;
}
