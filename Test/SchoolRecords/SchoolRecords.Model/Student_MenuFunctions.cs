using NakedFunctions;
using System;
using System.Linq;

namespace SchoolRecords.Model
{
public static class Student_MenuFunctions
{

        public static IQueryable<Student> AllStudents(IContext context) => context.Instances<Student>();

        public static IQueryable<Student> FindStudentByLastName(string lastName, IContext context) =>
             context.Instances<Student>().Where(s => s.LastName.ToUpper().Contains(lastName.ToUpper()));

        public static (Student, IContext) NewStudent(string firstName, string lastName, DateTime dob, IContext context)
        {
            var s = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dob
            };
            return (s, context.WithNew(s));
        }
    }
}
