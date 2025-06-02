using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class StorageLocation
{
    public int StorageLocationId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
