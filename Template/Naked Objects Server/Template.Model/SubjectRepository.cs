using NakedObjects;
using System.Linq;

namespace Template.Model
{
    [Named("Subjects")]
    public class SubjectRepository
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public Subject CreateNewSubject() => Container.NewTransientInstance<Subject>();

        public IQueryable<Subject> AllSubjects() => Container.Instances<Subject>();

        public IQueryable<Subject> FindSubjectByName(string name) => AllSubjects().Where(c => c.Name.ToUpper().Contains(name.ToUpper()));
    }
}
