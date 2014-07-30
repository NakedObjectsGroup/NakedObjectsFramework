using System.Data.Entity;

namespace TestCodeOnly {
    public class CodeFirstContext : DbContext {
        public CodeFirstContext(string dbName) : base("name=" + dbName) { }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CountryCode> CountryCodes { get; set; }
        public DbSet<DomesticAddress> DomesticAddresses { get; set; }
        public DbSet<InternationalAddress> InternationalAddresses { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}