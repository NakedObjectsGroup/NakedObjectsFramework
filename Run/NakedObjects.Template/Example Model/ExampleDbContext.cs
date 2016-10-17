
using System.Data.Entity;

namespace ExampleModel
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(string dbName) : base(dbName)
        {
            Database.SetInitializer<ExampleDbContext>(new DropCreateDatabaseIfModelChanges<ExampleDbContext>());
        }

        public DbSet<Customer> Customers { get; set; }
    }

}
