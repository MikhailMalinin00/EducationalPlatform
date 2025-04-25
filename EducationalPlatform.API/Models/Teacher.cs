using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Teacher
{
    public Guid Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public Guid RoleId { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<CompletedTasksComment> CompletedTasksComments { get; set; } = new List<CompletedTasksComment>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TeachersSubject> TeachersSubjects { get; set; } = new List<TeachersSubject>();
}
