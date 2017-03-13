using NakedObjects;
using System.Linq;


namespace Template.Model
{
    public class StudentRepository
    {
        #region Injected Services
        //An implementation of this interface is injected automatically by the framework
        public IDomainObjectContainer Container { set; protected get; }
        #endregion
        public Student CreateNewStudent()
        {
            return Container.NewTransientInstance<Student>();
        }

        public IQueryable<Student> AllStudents()
        {
            return Container.Instances<Student>();
        }

        public IQueryable<Student> FindStudentByName(string name)
        {
            return AllStudents().Where(c => c.FullName.ToUpper().Contains(name.ToUpper()));
        }
    }

}
