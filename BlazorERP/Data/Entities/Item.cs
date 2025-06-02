using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class Item
{
    public int ItemId { get; set; }

    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? UnitId { get; set; }

    public decimal DefaultCost { get; set; }

    public decimal DefaultPrice { get; set; }

    public int ReorderPoint { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedUtc { get; set; }

    public virtual ItemCategory Category { get; set; } = null!;

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<ItemCostHistory> ItemCostHistories { get; set; } = new List<ItemCostHistory>();

    public virtual ICollection<ItemImage> ItemImages { get; set; } = new List<ItemImage>();

    public virtual ICollection<ItemPriceHistory> ItemPriceHistories { get; set; } = new List<ItemPriceHistory>();

    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();

    public virtual ICollection<SalesOrderLine> SalesOrderLines { get; set; } = new List<SalesOrderLine>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual Unit? Unit { get; set; }
}
