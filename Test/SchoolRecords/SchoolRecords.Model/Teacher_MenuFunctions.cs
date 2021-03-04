using NakedFunctions;
using System.Linq;

namespace SchoolRecords.Model
{
public static class Teacher_MenuFunctions
{

        public static IQueryable<Teacher> AllTeachers(IContext context) => context.Instances<Teacher>();

        public static IQueryable<Teacher> FindTeacherByLastName(string lastName, IContext context) =>
            context.Instances<Teacher>().Where(t => t.LastName.ToUpper().Contains(lastName.ToUpper()));

        public static IQueryable<Teacher> FindTeacherByJobTitle(string job, IContext context) =>
            context.Instances<Teacher>().Where(t => t.JobTitle.ToUpper().Contains(job.ToUpper()));

        public static (Teacher, IContext) NewTeacher(string title, string lastName, string job, IContext context)
        {
            var t = new Teacher
            {
                Title = title,
                LastName = lastName,
                JobTitle = job
            };        
            return (t, context.WithNew(t));
        }
    }
}
