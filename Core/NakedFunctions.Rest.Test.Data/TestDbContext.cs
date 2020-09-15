using System.Data.Entity;

namespace NakedFunctions.Rest.Test.Data {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            context.SimpleRecords.Add(new SimpleRecord { Name = "Fred" });
            context.SaveChanges();
        }
    }


    public class TestDbContext : DbContext {
        public const string DatabaseName = "FunctionRestTests";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public TestDbContext() : base(Cs) { }

        public DbSet<SimpleRecord> SimpleRecords { get; set; }


        public static void Delete() => Database.Delete(Cs);

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }
}