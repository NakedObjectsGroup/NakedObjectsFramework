using NakedFunctions;
using System.Collections.Generic;

namespace SchoolRecords.Model
{
    public record Teacher
    {
        [Hidden]
        public virtual int Id { get; init; }

        [MemberOrder(1)]
        public virtual string Title { get; init; }

        [MemberOrder(2)]
        public virtual string LastName { get; init; }

        [MemberOrder(3)] //Optional
        public virtual string JobTitle { get; init; }

        [MemberOrder(4)]
        public virtual ICollection<Student> Tutees { get; init; } = new List<Student>();

        public override string ToString()
        {
            return $"{Title} {LastName}, {JobTitle}";
        }

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Teacher other) => ReferenceEquals(this, other);

    }
}
