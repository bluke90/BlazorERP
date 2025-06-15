using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class PurchaseOrder
{
    public int POId { get; set; }

    public int SupplierId { get; set; }

    public DateTime OrderedUtc { get; set; }

    public DateTime? ExpectedUtc { get; set; }

    public int? OrderStatusId { get; set; }

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();

    public virtual Supplier Supplier { get; set; } = null!;
}
