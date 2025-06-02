using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Aircraft
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Makemodel { get; set; }

    public int Totalseats { get; set; }

    public int Economyseats { get; set; }

    public int Businessseats { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
