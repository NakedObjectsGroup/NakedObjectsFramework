using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Template.Database
{
    public static class DatabaseConfig
    {       
        public static Func<IConfiguration, DbContext> EFCoreDbContextCreator =>
            c =>
            {
                var db = new ExampleDbContext(c.GetConnectionString("ExampleCS"));
                db.Create();
                return db;
            };

    }
}
