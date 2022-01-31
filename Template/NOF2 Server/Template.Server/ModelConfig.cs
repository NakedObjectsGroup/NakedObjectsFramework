using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Template.Database;
using Template.Model;

namespace NakedObjects.Rest.App.Demo
{
    public static class ModelConfig
    {
        public static Type[] DomainModelTypes() => new[] { typeof(Student), typeof(TeachingSet), typeof(Teacher), typeof(Subject), typeof(SubjectReport), typeof(Grades)};
        public static Type[] DomainModelServices() => new Type[] { };

        public static Type[] MainMenus() => new[] { typeof(Students), typeof(Sets), typeof(Teachers), typeof(Subjects) };
        
        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator => c => new ExampleDbContext(c.GetConnectionString("ExampleCS"));

    }
}
