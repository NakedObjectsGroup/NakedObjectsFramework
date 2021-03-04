using NakedFunctions;

namespace SchoolRecords.Model
{
    [Bounded]
    public record Subject
    {
        [Hidden]
        public virtual int Id { get; init; }

        [MemberOrder(1)]
        public virtual string Name { get; init; }

        public override string ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Subject other) => ReferenceEquals(this, other);

    }
}
