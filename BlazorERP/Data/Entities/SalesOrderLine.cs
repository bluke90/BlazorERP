using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class SalesOrderLine
{
    public long SOLineId { get; set; }

    public int SOId { get; set; }

    public int ItemId { get; set; }

    public int QtyOrdered { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual SalesOrder SO { get; set; } = null!;
}
