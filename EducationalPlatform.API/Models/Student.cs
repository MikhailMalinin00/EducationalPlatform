using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Student
{
    public Guid Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public Guid GroupId { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<CompletedTask> CompletedTasks { get; set; } = new List<CompletedTask>();

    public virtual ICollection<CompletedTasksComment> CompletedTasksComments { get; set; } = new List<CompletedTasksComment>();

    public virtual Group Group { get; set; } = null!;
}
