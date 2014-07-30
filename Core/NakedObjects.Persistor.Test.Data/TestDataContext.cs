using System.Collections.Specialized;
using System.Data.Entity;
using NakedObjects.Persistor.TestData;

namespace TestData {
    public class TestDataContext : DbContext {
        public TestDataContext() : base("name=TestDataCodeOnly")   {}
        //public DbSet<Address> Addresses { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderFail> OrderFails { get; set; }
    }
}