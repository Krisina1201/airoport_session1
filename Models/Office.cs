﻿using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class Office
{
    public int Id { get; set; }

    public int Countryid { get; set; }

    public string Title { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
