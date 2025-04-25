using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class CompletedTasksComment
{
    public Guid Id { get; set; }

    public Guid CompletedTaskId { get; set; }

    public Guid? StudentId { get; set; }

    public Guid? TeacherId { get; set; }

    public string? CommentText { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
