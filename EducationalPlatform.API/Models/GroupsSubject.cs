using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class GroupsSubject
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public Guid SubjectId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<SubjectSection> SubjectSections { get; set; } = new List<SubjectSection>();
}
