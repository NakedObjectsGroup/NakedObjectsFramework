using Microsoft.EntityFrameworkCore;


namespace Template.Model
{
    public class ExampleDbContext : DbContext
    {

        private readonly string cs;

        public ExampleDbContext(string cs) => this.cs = cs;

        public DbSet<Student> Students { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(new Student { Id = 1, FullName = "Alie Algol" });
            modelBuilder.Entity<Student>().HasData(new Student { Id = 2, FullName = "Forrest Fortran" });
            modelBuilder.Entity<Student>().HasData(new Student { Id = 3, FullName = "James Java" });
        }
       
            public void Delete() => Database.EnsureDeleted();

            public void Create() => Database.EnsureCreated();
    }
}
