using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class Stock
{
    public int ItemId { get; set; }

    public int StorageLocationId { get; set; }

    public int OnHand { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual StorageLocation StorageLocation { get; set; } = null!;
}
