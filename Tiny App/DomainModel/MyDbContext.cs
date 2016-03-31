
using System.Data.Entity;

namespace DomainModel
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(string dbName) : base(dbName)
        {
            Database.SetInitializer<DbContext>(new DropCreateDatabaseIfModelChanges<DbContext>());
        }

        public DbSet<Customer> Customers { get; set; }
    }

}
