using System;
using System.Collections.Generic;

namespace Airport.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public DateTime? Entrance { get; set; }

    public DateTime? Exit { get; set; }

    public string? Error { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
