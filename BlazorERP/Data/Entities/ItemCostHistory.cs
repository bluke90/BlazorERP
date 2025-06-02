using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class ItemCostHistory
{
    public long ItemCostId { get; set; }

    public int ItemId { get; set; }

    public decimal Cost { get; set; }

    public DateTime ChangedUtc { get; set; }

    public virtual Item Item { get; set; } = null!;
}
