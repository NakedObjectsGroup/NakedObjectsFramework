using NakedFunctions;
using System;

namespace SchoolRecords.Model
{
    public record SubjectReport 
    {
        [Hidden]
        public virtual int Id { get; init; }

        [MemberOrder(1)]
        public virtual Student Student { get; init; }

        [MemberOrder(2)]
        public virtual Subject Subject { get; init; }

        [MemberOrder(3)]
        public virtual string Grade { get; init; }

        [MemberOrder(4)]
        public virtual Teacher GivenBy { get; init; }

        [MemberOrder(5), Mask("d")]
        public virtual DateTime Date { get; init; }

        [MemberOrder(6), MultiLine]
        public virtual string Notes { get; init; }

        public override string ToString() => $"{Subject}, {Date}";

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(SubjectReport other) => ReferenceEquals(this, other);
    }
}
