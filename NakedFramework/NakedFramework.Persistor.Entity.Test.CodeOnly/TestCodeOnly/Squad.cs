using System.Collections.Generic;

namespace TestCodeOnly;

public class Squad {
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public virtual ICollection<System> Systems { get; set; } = new HashSet<System>();
}