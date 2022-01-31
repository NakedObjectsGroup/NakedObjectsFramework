using NakedObjects;
using System.Linq;

namespace Template.Model
{
    [Named("Students")]
    public class StudentRepository
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public Student CreateNewStudent() => Container.NewTransientInstance<Student>();

        public IQueryable<Student> AllStudents() => Container.Instances<Student>();

        public IQueryable<Student> FindStudentByName(string name) => AllStudents().Where(c => c.FullName.ToUpper().Contains(name.ToUpper()));

    }
}
