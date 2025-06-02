using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public int Aircraftid { get; set; }

    public int Routeid { get; set; }

    public decimal Economyprice { get; set; }

    public bool Confirmed { get; set; }

    public string? Flightnumber { get; set; }

    public virtual Aircraft Aircraft { get; set; } = null!;

    public virtual Route Route { get; set; } = null!;
}
