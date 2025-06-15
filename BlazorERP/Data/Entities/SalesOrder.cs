using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class SalesOrder
{
    public int SOId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderedUtc { get; set; }

    public int OrderStatusId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual ICollection<SalesOrderLine> SalesOrderLines { get; set; } = new List<SalesOrderLine>();
}
