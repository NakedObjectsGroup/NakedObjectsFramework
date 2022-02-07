using Microsoft.EntityFrameworkCore;
using Template.Model;

namespace Template.Database
{
    public class ExampleDbContext : DbContext
    {

        private readonly string cs;

        public ExampleDbContext(string cs) => this.cs = cs;

        public DbSet<Student> Students { get; set; }
        public DbSet<Student> Sets { get; set; }
        public DbSet<Student> Teachers { get; set; }
        public DbSet<Student> Subjects { get; set; }
        public DbSet<SubjectReport> SubjectReports { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().Map();
            modelBuilder.Entity<TeachingSet>().Map();
            modelBuilder.Entity<SubjectReport>().Map();
            modelBuilder.Entity<Teacher>().Map();
            SeedData.CreateSeedData(modelBuilder);
        }

        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();


    }
}
