using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class User
{
    public int Id { get; set; }

    public int Roleid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Firstname { get; set; }

    public string Lastname { get; set; } = null!;

    public int? Officeid { get; set; }

    public DateOnly? Birthdate { get; set; }

    public bool? Active { get; set; }

    public virtual Office? Office { get; set; }

    public virtual Role Role { get; set; } = null!;
}
