using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
