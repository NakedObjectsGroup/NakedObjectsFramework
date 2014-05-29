using System.Data.Entity;

namespace $rootnamespace$
{
    public class $safeitemname$ : DbContext {
        public $safeitemname$(string name) : base(name) {}
        public $safeitemname$() {}

        //Add DbSet properties for root objects, thus:
        //public DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {	
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