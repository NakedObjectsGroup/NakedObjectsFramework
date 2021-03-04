using System.Collections.Generic;
using System.Linq;
using NakedFunctions;

namespace SchoolRecords.Model
{
    public static class TeachingSet_Functions
    {
        public static IContext AddStudentToSet(this TeachingSet ts, Student student, IContext context) {
            var students = ts.Students;
            students.Add(student);
            return context.WithUpdated(ts, ts with { Students =  students});
        }

        public static IQueryable<Student> AutoComplete1AddStudentToSet(this TeachingSet ts,
            [MinLength(3), DescribedAs("Last name")] string name, IContext context) =>
            Student_MenuFunctions.FindStudentByLastName(name, context);

        public static IContext RemoveStudentFromSet(this TeachingSet ts, Student student, IContext context)
        {
            var students = ts.Students;
            students.Remove(student);
            return context.WithUpdated(ts, ts with { Students = students });
        }

        public static IList<Student> Choices0RemoveStudentFromSet(this TeachingSet ts) => ts.Students.ToList();
    }
}

