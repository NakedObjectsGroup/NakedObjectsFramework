using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using Template.Database;
using Template.Model;

namespace Template.Server
{
    public static class ModelConfig
    {
        public static Type[] DomainModelTypes() => Assembly.GetAssembly(typeof(Student)).GetTypes().
                     Where(t => t.IsPublic && t.Namespace == "Template.Model").
                     ToArray();

        public static Type[] DomainModelServices() => new Type[] { };

        public static Type[] MainMenus() => new[] { typeof(Students), typeof(Sets), typeof(Teachers), typeof(Subjects) };
        
        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator =>
            c =>
            {
                var db = new ExampleDbContext(c.GetConnectionString("ExampleCS"));
                db.Create();
                return db;
            };

    }
}
