
using System.Data.Entity;
using Template.Model;

namespace Template.DataBase
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(string dbName) : base(dbName)
        {
            Database.SetInitializer<DbContext>(new DropCreateDatabaseIfModelChanges<DbContext>());
        }

        public DbSet<Customer> Customers { get; set; }
    }

}
