using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Template.Model
{
    public static class ModelConfig
    {
        public static Type[] DomainModelTypes() => new[] { typeof(Student) };

        public static Type[] DomainModelServices() => new[] { typeof(ExampleService) };

        public static Type[] MainMenus() => new[] { typeof(ExampleService) };

        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator =>
            c =>
            {
                var db = new ExampleDbContext(c.GetConnectionString("ExampleCS"));
                db.Create();
                return db;
            };
    }
}
