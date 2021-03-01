using NakedFunctions;
using System.Linq;
using Template.Model.Types;

namespace Template.Model.Functions
{
    [Named("Students")]
    public static class Student_MenuFunctions
    {
        public static (Student, IContext) CreateNewStudent(string fullName, IContext context)
        {
            var s = new Student { FullName = fullName };
            return (s, context.WithNew(s));
        }       

        public static IQueryable<Student> AllStudents(IContext context) =>
            context.Instances<Student>();

        public static IQueryable<Student> FindStudentByName(string name, IContext context) =>
            context.Instances<Student>().Where(c => c.FullName.ToUpper().Contains(name.ToUpper()));

    }

}
