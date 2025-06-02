using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Route
{
    public int Id { get; set; }

    public int Departureairportid { get; set; }

    public int Arrivalairportid { get; set; }

    public int Distance { get; set; }

    public int Flighttime { get; set; }

    public virtual Airport Arrivalairport { get; set; } = null!;

    public virtual Airport Departureairport { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
