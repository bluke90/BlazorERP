using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class ItemCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
