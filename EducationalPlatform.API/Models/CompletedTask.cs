using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class CompletedTask
{
    public Guid Id { get; set; }

    public Guid AssignmentId { get; set; }

    public Guid StudentId { get; set; }

    public DateTime? ExecutionDateTime { get; set; }

    public string? AttachedDocument { get; set; }

    public int? Score { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
