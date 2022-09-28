using Microsoft.EntityFrameworkCore;

namespace Template.Model;

public class ExampleDbContext : DbContext {
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
        : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Student> Sets { get; set; }
    public DbSet<Student> Teachers { get; set; }
    public DbSet<Student> Subjects { get; set; }
    public DbSet<SubjectReport> SubjectReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        SeedData.CreateSeedData(modelBuilder);
    }

    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}