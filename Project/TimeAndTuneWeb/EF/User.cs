using System;
using System.Collections.Generic;

namespace EFCore;

public partial class User
{
    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Coinsamount { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
