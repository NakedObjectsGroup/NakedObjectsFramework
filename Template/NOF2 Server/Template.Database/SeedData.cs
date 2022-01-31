using Microsoft.EntityFrameworkCore;
using System;
using Template.Model;

namespace Template.Database
{
    public static class SeedData
    {

        public static void CreateSeedData(ModelBuilder modelBuilder)
        {

            #region  Teachers
            var br = CreateNewTeacher(1, "Mr. Deckerd", modelBuilder);
            var dt = CreateNewTeacher(2, "Dr. Tyrell", modelBuilder);
            var mm = CreateNewTeacher(3, "Maj. Major", modelBuilder);
            var md = CreateNewTeacher(4, "Mrs. Doubtfire", modelBuilder);
            var dd = CreateNewTeacher(5, "Dr. Doolittle", modelBuilder);
            var ds = CreateNewTeacher(6, "Dr. Strangelove", modelBuilder);
            var mi = CreateNewTeacher(7, "Ms. Issippi", modelBuilder);
            var ma = CreateNewTeacher(8, "Ms. Andrist", modelBuilder);
            var dj = CreateNewTeacher(9, "Dr. Jekyll", modelBuilder);
            var mh = CreateNewTeacher(10, "Mr. Hyde", modelBuilder);
            var mr = CreateNewTeacher(11, "Mrs. Robinson", modelBuilder);
            var mw = CreateNewTeacher(12, "Mrs. Worthington", modelBuilder);
            var dh = CreateNewTeacher(13, "Dr. Hu", modelBuilder);
            var co = CreateNewTeacher(14, "Cpt. Over", modelBuilder);
            #endregion
            #region Students
            var aa = CreateNewStudent(1, "Alie Algol", 12, modelBuilder);
            var ff = CreateNewStudent(2, "Forrest Fortran", 12, modelBuilder);
            var jj = CreateNewStudent(3, "James Java", 12, modelBuilder);
            var cc = CreateNewStudent(4, "Celia Cee-Sharp", 12, modelBuilder);
            var vv = CreateNewStudent(5, "Veronica Vee-Bee", 13, modelBuilder);
            var ss = CreateNewStudent(6, "Simon SmallTalk", 13, modelBuilder);
            var tt = CreateNewStudent(7, "Tilly TypeScript", 13, modelBuilder);
            var pp = CreateNewStudent(8, "Petra Python", 11, modelBuilder);
            var hh = CreateNewStudent(9, "Harry Haskell", 10, modelBuilder);
            var cb = CreateNewStudent(10, "Corinie Cobol", 11, modelBuilder);
            #endregion
            #region Subjects
            var csc = CreateNewSubject(1, "Computer Science", modelBuilder);
            var math = CreateNewSubject(2, "Maths", modelBuilder);
            var eng = CreateNewSubject(3, "English", modelBuilder);
            var phy = CreateNewSubject(4, "Physics", modelBuilder);
            var chem = CreateNewSubject(5, "Chemistry", modelBuilder);
            var bio = CreateNewSubject(6, "Biology", modelBuilder);
            var his = CreateNewSubject(7, "History", modelBuilder);
            var fre = CreateNewSubject(8, "French", modelBuilder);
            var ger = CreateNewSubject(9, "German", modelBuilder);
            var dra = CreateNewSubject(10,"Drama", modelBuilder);
            var des = CreateNewSubject(11,"Design & Technology", modelBuilder);
            var film = CreateNewSubject(12, "Film Studies", modelBuilder);
            var geo = CreateNewSubject(13, "Geography", modelBuilder);
            #endregion
            #region Sets
            //var CS12 = CreateNewSet("CS12", csc, 12, ds, modelBuilder);
            //var CS13 = CreateNewSet("CS13", csc, 13, ds, modelBuilder);
            //var MA09_1 = CreateNewSet("MA09_1", math, 9, ma, modelBuilder);
            //var MA10_1 = CreateNewSet("MA10_1", math, 10, ma, modelBuilder);
            //var MA11_1 = CreateNewSet("MA11_1", math, 11, ma, modelBuilder);
            //var MA09_2 = CreateNewSet("MA09_2", math, 9, dj, modelBuilder);
            //var MA10_2 = CreateNewSet("MA10_2", math, 10, dj, modelBuilder);
            //var MA11_2 = CreateNewSet("MA11_2", math, 11, dj, modelBuilder);
            #endregion
        }

        private static Teacher CreateNewTeacher(int id, string name, ModelBuilder modelBuilder)
        {
            var t = new Teacher() { Id = id, mappedFullName = name };
            modelBuilder.Entity<Teacher>().HasData(t);
            return t;
        }

        private static Student CreateNewStudent(int id, string name, int year, ModelBuilder modelBuilder)
        {
            var stu = new Student() { Id = id, mappedFullName = name, mappedCurrentYearGroup = year };
            modelBuilder.Entity<Student>().HasData(stu);
            return stu;
        }

        private static Subject CreateNewSubject(int id, string name, ModelBuilder modelBuilder)
        {
            var sub = new Subject() { Id = id, mappedName = name };
            modelBuilder.Entity<Subject>().HasData(sub);
            return sub;
        }

        private static TeachingSet CreateNewSet(int id, string name, Subject subject, int yearGroup, Teacher teacher, ModelBuilder modelBuilder)
        {
            var set = new TeachingSet() { Id = id, mappedSetName = name, Subject = subject, mappedYearGroup = yearGroup, Teacher = teacher };
            modelBuilder.Entity<TeachingSet>().HasData(set);
            return set;
        }


    }
}
