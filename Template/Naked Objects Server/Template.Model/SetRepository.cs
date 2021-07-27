using NakedObjects;
using System.Linq;

namespace Template.Model
{
    [Named("Sets")]
    public class SetRepository
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public Set CreateNewSet() => Container.NewTransientInstance<Set>();

        public IQueryable<Set> ListSets([Optionally] Subject subject, [Optionally] int? yearGroup)
        {
            var sets = Container.Instances<Set>();
            if (subject != null)
            {
                int id = subject.Id;
                sets = sets.Where(s => s.Subject.Id == id);
            }
            if (yearGroup != null)
            {
                sets = sets.Where(s => s.YearGroup == yearGroup.Value);
            }
            return sets.OrderBy(s => s.YearGroup).ThenBy(s => s.Subject.Name);
        }
    }
}
