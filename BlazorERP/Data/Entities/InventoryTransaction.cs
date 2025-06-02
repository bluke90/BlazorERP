using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class InventoryTransaction
{
    public long TxnId { get; set; }

    public int ItemId { get; set; }

    public int StorageLocationId { get; set; }

    public int QtyChange { get; set; }

    public string TxnType { get; set; } = null!;

    public string? RefDocument { get; set; }

    public decimal? PerUnitCost { get; set; }

    public DateTime CreatedUtc { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual StorageLocation StorageLocation { get; set; } = null!;
}
