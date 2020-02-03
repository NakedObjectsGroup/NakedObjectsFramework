using System.Data.Entity;

namespace SimpleDatabase {

    public class SimpleDatabaseDbContext : DbContext {
        public SimpleDatabaseDbContext(string name) : base(name) { }
        public SimpleDatabaseDbContext() { }

        //Add DbSet properties for root objects, thus:
        public DbSet<Person> Persons { get; set; }
        public DbSet<Fruit> Fruits { get; set; }
        public DbSet<Food> Foods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //Initialisation
            //Use the Naked Objects > DbInitialiser template to add an initialiser, then reference thus:
            

            //Mappings
            //Use the Naked Objects > DbMapping template to create mapping classes & reference them thus:
            //modelBuilder.Configurations.Add(new EmployeeMapping());
            //modelBuilder.Configurations.Add(new CustomerMapping());
        }
    }
}