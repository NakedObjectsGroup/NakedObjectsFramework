using NakedFunctions;

namespace Template.Model.Types
{
    public record Student
    {
        [Hidden]
        public virtual int Id { get; init; }

        public virtual string FullName { get; init; }

        public override string ToString() => FullName;

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Student other) => ReferenceEquals(this, other);
    }
}
