using System.Collections.Generic;

namespace TestCodeOnly;

public partial class System
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public virtual ICollection<Squad> Squads { get; set; } = new HashSet<Squad>();
}