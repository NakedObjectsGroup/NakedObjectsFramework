using System;
using System.Data.Entity;

namespace SchoolRecords.Model
{
    public class Initializer : DropCreateDatabaseAlways<DatabaseContext>
    {

protected override void Seed(DatabaseContext context)
{             
    var students = context.Students;
    var alg = NewStudent(students, "Alie", "Algol", "19/02/2004", "HM287");
    var frt = NewStudent(students, "Forrest", "Fortran", "22/09/2003", "LX046");
    var jav = NewStudent(students, "James", "Java", "24/03/2004", "HW531");
    var cee = NewStudent(students, "Celia", "Cee-Sharp", "12/09/2003", "LX033");
    var vee= NewStudent(students, "Veronica", "Vee-Bee", "05/09/2003", "HM119");
    var sim = NewStudent(students, "Simon", "Simula", "31/07/2003", "HW309");
    var typ = NewStudent(students, "Tilly", "TypeScript", "14/01/2003", "LX008");
    var pyt = NewStudent(students, "Petra", "Python", "17/06/2003", "LX 144");
    var has = NewStudent(students, "Harry", "Haskell", "08/04/2003", "HM200");
    var cob = NewStudent(students, "Corinie","Cobol", "28/02/2003", "HW442");

            var teachers = context.Teachers;
            var dec = NewTeacher(teachers, "Mr.","Deckerd");
            var tyr = NewTeacher(teachers, "Dr.", "Tyrell");
            var maj = NewTeacher(teachers, "Maj.", "Major");
            var dou = NewTeacher(teachers, "Mrs.", "Doubtfire");
            var doo = NewTeacher(teachers, "Dr.", "Doolittle");
            var str = NewTeacher(teachers, "Dr.", "Strangelove");
            var iss = NewTeacher(teachers, "Ms.", "Issippi");
            var and = NewTeacher(teachers, "Ms.", "Andrist");
            var jek = NewTeacher(teachers, "Dr.", "Jekyll");
            var hyd = NewTeacher(teachers, "Mr.", "Hyde");
            var rob = NewTeacher(teachers, "Mrs.", "Robinson");
            var wor = NewTeacher(teachers, "Mrs.", "Worthington");
            var hu = NewTeacher(teachers, "Dr.", "Hu");
            var ove = NewTeacher(teachers, "Cpt.", "Over");

            //alg.Tutor = dec;
            //frt.Tutor = tyr;
            //jav.Tutor = maj;

        var subjects = context.Subjects;
        var csc = CreateNewSubject(subjects, "Computer Science");
        var math = CreateNewSubject(subjects, "Maths");
        var eng = CreateNewSubject(subjects, "English");
        var phy = CreateNewSubject(subjects, "Physics");
        var chem = CreateNewSubject(subjects, "Chemistry");
        var bio = CreateNewSubject(subjects, "Biology");
        var his = CreateNewSubject(subjects, "History");
        var fre = CreateNewSubject(subjects, "French");
        var ger = CreateNewSubject(subjects, "German");

        var sets = context.Sets;
        var CS12 = CreateNewSet(sets, "CS12", csc, 12, dec);
        var CS13 = CreateNewSet(sets, "CS13", csc, 13, dec);
        var MA09_1 = CreateNewSet(sets, "MA09_1", math, 9, rob);
        var MA10_1 = CreateNewSet(sets, "MA10_1", math, 10, rob);
        var MA11_1 = CreateNewSet(sets, "MA11_1", math, 11, hu);
        var MA09_2 = CreateNewSet(sets, "MA09_2", math, 9, dou);
        var MA10_2 = CreateNewSet(sets, "MA10_2", math, 10, dou);
        var MA11_2 = CreateNewSet(sets, "MA11_2", math, 11, dou);

        AssignStudents(CS12, alg, cee, frt);
        AssignStudents(CS13, vee, sim);

            //TODO: Add all created objects to DbSets & Save changes
        }

        private Student NewStudent(DbSet<Student> students, string firstName, string lastName, string dob, string number)
        {
            var s = new Student() {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = Convert.ToDateTime(dob),
                StudentNumber = number,
            };
            return s;
        }

    private Teacher NewTeacher(DbSet<Teacher> teachers, string title, string lastName)
    {
        var t = new Teacher();
            //TODO
        //t.Title = title;
        //t.LastName = lastName;
        //teachers.Add(t);
        return t;
    }

        private Subject CreateNewSubject(DbSet<Subject> subjects, string name)
        {
            var obj = new Subject() { Name = name };
            subjects.Add(obj);
            return obj;
        }

        private TeachingSet CreateNewSet(DbSet<TeachingSet> sets, string name, Subject subject, int yearGroup, Teacher teacher)
        {
            var obj = new TeachingSet() { SetName = name, Subject = subject, YearGroup = yearGroup, Teacher = teacher };
            sets.Add(obj);
            return obj;
        }

        private void AssignStudents(TeachingSet set, params Student[] students)
        {
            foreach (Student stu in students)
            {
                set.Students.Add(stu);
            }
        }
    }
}
