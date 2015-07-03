using System.Data.Entity;

namespace ModelFirst {

    public class ModefFirstInitializer : DropCreateDatabaseAlways<ModelFirstDbContext> {
        protected override void Seed(ModelFirstDbContext context) {

            var food = new Food() {Name = "Steak"};
            var fruit = new Fruit() {Name = "Apple", Organic = true};

            var p1 = new Person() {  ComplexProperty = new NameType() {Firstname = "Fred", Surname = "Jones"}, ComplexProperty_1 = new ComplexType1 {s1 = "a", s2 = "b"}};
            var p2 = new Person() { ComplexProperty = new NameType() { Firstname = "Tom", Surname = "Smith" }, ComplexProperty_1 = new ComplexType1 { s1 = "c", s2 = "d" } };

            p1.Food.Add(fruit);
            p2.Food.Add(food);


            context.Persons.Add(p1);

            context.Persons.Add(p2);

        }
    }


    public class ModelFirstDbContext : DbContext {
        public ModelFirstDbContext(string name) : base(name) { }
        public ModelFirstDbContext() { }

        //Add DbSet properties for root objects, thus:
        public DbSet<Person> Persons { get; set; }
        public DbSet<Fruit> Fruits { get; set; }
        public DbSet<Food> Foods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //Initialisation
            //Use the Naked Objects > DbInitialiser template to add an initialiser, then reference thus:
            Database.SetInitializer(new ModefFirstInitializer()); 

            //Mappings
            //Use the Naked Objects > DbMapping template to create mapping classes & reference them thus:
            //modelBuilder.Configurations.Add(new EmployeeMapping());
            //modelBuilder.Configurations.Add(new CustomerMapping());
        }
    }
}