using System;
using System.Collections.Generic;

namespace BlazorERP.Data.Entities;

public partial class Unit
{
    public int UnitId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
