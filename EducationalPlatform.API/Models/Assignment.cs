using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Assignment
{
    public Guid Id { get; set; }

    public string? AssignmentName { get; set; }

    public string? Description { get; set; }

    public DateOnly? DueDate { get; set; }

    public Guid SubjectSectionId { get; set; }

    public string? AttachedDocument { get; set; }

    public virtual ICollection<CompletedTask> CompletedTasks { get; set; } = new List<CompletedTask>();

    public virtual SubjectSection SubjectSection { get; set; } = null!;
}
