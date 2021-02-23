using NakedFunctions;

namespace Template.Model.Types
{
    public record Student
    {
        //All persisted properties on a record must be 'virtual'

        [Hidden]//Indicates that this property will never be seen in the UI
        public virtual int Id { get; init; }

        public virtual string FullName { get; init; }

        [Versioned] //Used by Naked Functions to check if object state is up to date before initiating any action upon it
        public virtual byte[] TimeStamp { get; init; }

        //Defines the title of the object when presented to the user
        public override string ToString() => FullName;

        //In C# 9 the following boilerplate must be added to each record definition to
        //enable it to work correctly with Entity Framework
        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Student other) => ReferenceEquals(this, other);
    }
}
