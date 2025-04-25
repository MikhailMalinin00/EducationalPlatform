using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class TeachersSubject
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public Guid SubjectId { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
