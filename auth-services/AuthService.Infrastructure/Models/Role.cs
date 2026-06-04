using System;
using System.Collections.Generic;

namespace auth_services.AuthService.Infrastructure.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
