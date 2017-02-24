
using System.Data.Entity;

namespace ExampleModel
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
