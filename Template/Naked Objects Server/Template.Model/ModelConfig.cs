using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Template.Model
{
    public static class ModelConfig
    {
        public static Type[] DomainModelTypes() => new[] { typeof(Student), typeof(Set), typeof(Teacher), typeof(Subject), typeof(SubjectReport), typeof(Grades)};

        public static Type[] DomainModelServices() => new[] { typeof(StudentRepository), typeof(SetRepository), typeof(TeacherRepository), typeof(SubjectRepository) };

        public static Type[] MainMenus() => new[] { typeof(StudentRepository), typeof(SetRepository), typeof(TeacherRepository), typeof(SubjectRepository) };

        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator =>
            c =>
            {
                var db = new ExampleDbContext(c.GetConnectionString("ExampleCS"));
                db.Create();
                return db;
            };
    }
}
