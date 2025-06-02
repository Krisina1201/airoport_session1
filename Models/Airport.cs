using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Airport
{
    public int Id { get; set; }

    public int Countryid { get; set; }

    public string Iatacode { get; set; } = null!;

    public string? Name { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Route> RouteArrivalairports { get; set; } = new List<Route>();

    public virtual ICollection<Route> RouteDepartureairports { get; set; } = new List<Route>();
}
