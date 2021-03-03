using System;
using System.Linq;
using NakedFunctions;

namespace SchoolRecords.Model
{
    public static class Student_Functions
    {
        public static IContext ConfirmDateOfBirth(this Student s, DateTime dateOfBirth, IContext context)
        {
            var message = dateOfBirth == s.DateOfBirth ? "CORRECT" : "INCORRECT";
            return context.WithInformUser($"The date of birth you entered is {message} for this student.");
        }

        public static string ValidateDateOfBirth(this Student s, DateTime dob) =>  
            dob > DateTime.Today ? "Date of Birth cannot be after today" : null;
    

        public static IQueryable<Teacher> AutoCompleteTutor(this Student s, [MinLength(3)] string match, IContext context) =>
            Teacher_MenuFunctions.FindTeacherByLastName(match, context);

        internal static int Age(Student s)
        {
            var today = DateTime.Today;
            int age = today.Year - s.DateOfBirth.Year;

            if (today.Month < s.DateOfBirth.Month || (today.Month == s.DateOfBirth.Month && today.Day < s.DateOfBirth.Day))
                age--;
            return age;
        }


        public  static IQueryable<SubjectReport> ListRecentReports(this Student s, IContext context)
        {
            int id = s.Id;
            return context.Instances<SubjectReport>().Where(sr => sr.Student.Id == id).OrderByDescending(sr => sr.Date);
        }

        public static (SubjectReport, IContext) CreateNewReport(this Student s, IContext context)
        {
            var rep = new SubjectReport { Student = s };
            return (rep, context.WithNew(rep));
        }
    }
}
