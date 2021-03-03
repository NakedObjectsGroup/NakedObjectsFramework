using System.Collections.Generic;
using NakedFunctions;

namespace SchoolRecords.Model
{
    public record TeachingSet
    {
        [Hidden]
        public virtual int Id { get; init; }

        [MemberOrder(1)]
        public virtual string SetName { get; init; }

        [MemberOrder(2)]
        public virtual Subject Subject { get; init; }

        [MemberOrder(3)] // Range(9,13)
        public virtual int YearGroup { get; init; }

        [MemberOrder(4)]
        public virtual Teacher Teacher { get; init; }

        [MemberOrder(5)]
        public virtual ICollection<Student> Students { get; init; } = new List<Student>();

        public override string ToString() =>  SetName;

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(TeachingSet other) => ReferenceEquals(this, other);

    }
}

