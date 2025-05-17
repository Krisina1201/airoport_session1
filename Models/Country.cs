using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Office> Offices { get; set; } = new List<Office>();
}
