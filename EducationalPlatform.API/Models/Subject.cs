using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Subject
{
    public Guid Id { get; set; }

    public string? SubjectName { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<GroupsSubject> GroupsSubjects { get; set; } = new List<GroupsSubject>();

    public virtual ICollection<TeachersSubject> TeachersSubjects { get; set; } = new List<TeachersSubject>();
}
