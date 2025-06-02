using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class ItemPriceHistory
{
    public long ItemPriceId { get; set; }

    public int ItemId { get; set; }

    public decimal Price { get; set; }

    public DateTime ChangedUtc { get; set; }

    public virtual Item Item { get; set; } = null!;
}
