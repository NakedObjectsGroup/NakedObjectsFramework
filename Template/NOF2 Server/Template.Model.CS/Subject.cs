using NakedObjects;

namespace Template.Model
{
    [Bounded]
    public class Subject
    {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        [Hidden]
        public virtual int Id { get; set; }

        [MemberOrder(1)]
        public virtual string Name { get; set; }

        public override string ToString() => Name;
    }
}
