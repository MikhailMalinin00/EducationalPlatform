using System;
using System.Collections.Generic;

namespace EducationalPlatform.API.Models;

public partial class ProgramTrack
{
    public Guid Id { get; set; }

    public string? ProgramTrackName { get; set; }

    public string? ShortPtname { get; set; }

    public bool? Visibility { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
