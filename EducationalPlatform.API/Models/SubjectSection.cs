using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class SubjectSection
{
    public Guid Id { get; set; }

    public string? SubjectSectionName { get; set; }

    public Guid GroupSubjectId { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual GroupsSubject GroupSubject { get; set; } = null!;
}
