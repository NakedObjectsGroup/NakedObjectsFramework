
using System.Data.Entity;
using Template.Model;

namespace Template.DataBase
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(string dbName) : base(dbName)
        {
            Database.SetInitializer(new ExampleDbInitializer());
        }

        public DbSet<Student> Students { get; set; }
    }

}
