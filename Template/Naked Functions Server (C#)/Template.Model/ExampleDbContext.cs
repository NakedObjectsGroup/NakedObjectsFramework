using Microsoft.EntityFrameworkCore;
using Template.Model.Types;

namespace Template.Model;

public class ExampleDbContext : DbContext {
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
        : base(options) { }

    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Student>().HasData(new Student { Id = 1, FullName = "Alie Algol" });
        modelBuilder.Entity<Student>().HasData(new Student { Id = 2, FullName = "Forrest Fortran" });
        modelBuilder.Entity<Student>().HasData(new Student { Id = 3, FullName = "James Java" });
    }

    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}