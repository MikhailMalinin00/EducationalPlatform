using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class Group
{
    public Guid Id { get; set; }

    public string? GroupName { get; set; }

    public Guid ProgramTrackId { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<GroupsSubject> GroupsSubjects { get; set; } = new List<GroupsSubject>();

    public virtual ProgramTrack ProgramTrack { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
