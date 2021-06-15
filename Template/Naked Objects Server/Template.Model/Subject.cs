using NakedObjects;

namespace Template.Model
{
    public class Subject
    {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        [Hidden]
        public virtual int Id { get; set; }

        [MemberOrder(1), Title]
        public virtual string Name { get; set; }
    }
}
