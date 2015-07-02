using System.Data.Entity;

namespace ModelFirst {
    public class ModelFirstDbContext : DbContext {
        public ModelFirstDbContext(string name) : base(name) { }
        public ModelFirstDbContext() { }

        //Add DbSet properties for root objects, thus:
        public DbSet<Food> Foods { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Fruit> Fruits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //Initialisation
            //Use the Naked Objects > DbInitialiser template to add an initialiser, then reference thus:
            //Database.SetInitializer(new MyDbInitialiser()); 

            //Mappings
            //Use the Naked Objects > DbMapping template to create mapping classes & reference them thus:
            //modelBuilder.Configurations.Add(new EmployeeMapping());
            //modelBuilder.Configurations.Add(new CustomerMapping());
        }
    }
}