using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class PurchaseOrderLine
{
    public long POLineId { get; set; }

    public int POId { get; set; }

    public int ItemId { get; set; }

    public int QtyOrdered { get; set; }

    public decimal UnitCost { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual PurchaseOrder PO { get; set; } = null!;
}
